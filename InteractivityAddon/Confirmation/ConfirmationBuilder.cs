using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Interactivity.Extensions;

namespace Interactivity.Confirmation
{
    /// <summary>
    /// The <see cref="ConfirmationBuilder"/> is used to create a <see cref="Confirmation"/>
    /// </summary>
    public sealed class ConfirmationBuilder
    {
        /// <summary>
        /// Gets or sets the content to be displayed in the <see cref="Confirmation"/>
        /// </summary>
        public PageBuilder Content { get; set; } = new PageBuilder();

        /// <summary>
        /// Gets or sets the users allowed to interact with the <see cref="Confirmation"/>.
        /// </summary>
        public List<SocketUser> Users { get; set; } = new List<SocketUser>();

        /// <summary>
        /// Gets or sets the <see cref="IEmote"/> to confirm the <see cref="Confirmation"/>.
        /// </summary>
        public IEmote ConfirmEmote { get; set; } = new Emoji("✅");

        /// <summary>
        /// Gets or sets the <see cref="IEmote"/> to decline the <see cref="Confirmation"/>.
        /// </summary>
        public IEmote DeclineEmote { get; set; } = new Emoji("❌");

        /// <summary>
        /// Gets or sets the <see cref="Embed"/> the <see cref="Confirmation"/> gets modified to on timeout.
        /// </summary>
        public EmbedBuilder TimeoutedEmbed { get; set; } = new EmbedBuilder().WithColor(Color.Red).WithTitle("Timed out! :alarm_clock:");

        /// <summary>
        /// Gets or sets the <see cref="Embed"/> the <see cref="Confirmation"/> gets modified to on cancel.
        /// </summary>
        public EmbedBuilder CancelledEmbed { get; set; } = new EmbedBuilder().WithColor(Color.Orange).WithTitle("Cancelled! :thumbsup:");

        /// <summary>
        /// Gets or sets what the <see cref="Confirmation"/> should delete.
        /// Valid will delete all reactions after a result has been captured.
        /// </summary>
        public DeletionOptions Deletion { get; set; } = DeletionOptions.AfterCapturedContext | DeletionOptions.Invalids | DeletionOptions.Valid;

        internal IEmote[] Emotes => new IEmote[] { ConfirmEmote, DeclineEmote };

        public Confirmation Build()
            => new Confirmation(
                Content?.Build() ?? throw new ArgumentNullException(nameof(Content)),
                Users?.AsReadOnlyCollection() ?? throw new ArgumentNullException(nameof(Users)),
                ConfirmEmote ?? throw new ArgumentNullException(nameof(ConfirmEmote)),
                DeclineEmote ?? throw new ArgumentNullException(nameof(DeclineEmote)),
                TimeoutedEmbed?.Build(),
                CancelledEmbed?.Build(),
                Deletion);

        /// <summary>
        /// Sets the content to be displayed in the <see cref="Confirmation"/>
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ConfirmationBuilder WithContent(PageBuilder content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// Sets the users allowed to interact with the <see cref="Confirmation"/>.
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        public ConfirmationBuilder WithUsers(params SocketUser[] users)
        {
            Users = users.ToList();
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IEmote"/> to confirm the <see cref="Confirmation"/>.
        /// </summary>
        /// <param name="confirmEmote"></param>
        /// <returns></returns>
        public ConfirmationBuilder WithConfirmEmote(IEmote confirmEmote)
        {
            ConfirmEmote = confirmEmote;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="IEmote"/> to decline the <see cref="Confirmation"/>.
        /// </summary>
        /// <param name="declineEmote"></param>
        /// <returns></returns>
        public ConfirmationBuilder WithDeclineEmote(IEmote declineEmote)
        {
            DeclineEmote = declineEmote;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Embed"/> the <see cref="Confirmation"/> gets modified to on timeout.
        /// </summary>
        /// <param name="timeoutedEmbed"></param>
        /// <returns></returns>
        public ConfirmationBuilder WithTimeoutedEmbed(EmbedBuilder timeoutedEmbed)
        {
            TimeoutedEmbed = timeoutedEmbed;
            return this;
        }

        /// <summary>
        /// Sets the <see cref="Embed"/> the <see cref="Confirmation"/> gets modified to on cancel.
        /// </summary>
        /// <param name="cancelledEmbed"></param>
        /// <returns></returns>
        public ConfirmationBuilder WithCancelledEmbed(EmbedBuilder cancelledEmbed)
        {
            CancelledEmbed = cancelledEmbed;
            return this;
        }

        /// <summary>
        /// Sets what the <see cref="Confirmation"/> should delete.
        /// </summary>
        /// <param name="deletion"></param>
        /// <returns></returns>
        public ConfirmationBuilder WithDeletion(DeletionOptions deletion)
        {
            Deletion = deletion;
            return this;
        }
    }
}