using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using Interactivity.Extensions;

namespace Interactivity.Pagination
{
    /// <summary>
    /// Represents a builder class for making a <see cref="StaticPaginator"/>.
    /// </summary>
    public sealed class StaticPaginatorBuilder : PaginatorBuilder
    {
        /// <summary>
        /// Gets or sets the pages of the <see cref="Paginator"/>.
        /// </summary>
        public IList<PageBuilder> Pages { get; set; }

        public override Paginator Build(int startPage = 0)
        {
            if (Pages.Count == 0)
            {
                throw new InvalidOperationException("A paginator needs at least one page!");
            }

            if (Emotes.Count == 0)
            {
                WithDefaultEmotes();
            }

            for (int i = 0; i < Pages.Count; i++)
            {
                Pages[i].WithPaginatorFooter(Footer, i, Pages.Count - 1, Users);
            }

            return new StaticPaginator(Users?.AsReadOnlyCollection() ?? throw new ArgumentNullException(nameof(Users)),
                                       Emotes?.AsReadOnlyDictionary() ?? throw new ArgumentNullException(nameof(Emotes)),
                                       CancelledEmbed?.Build(),
                                       TimeoutedEmbed?.Build(),
                                       Deletion,
                                       Pages?.Select(x => x.Build()).AsReadOnlyCollection() ?? throw new ArgumentNullException(nameof(Pages)),
                                       startPage);
        }

        #region WithValue

        /// <summary>
        /// Sets the users who can interact with the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithUsers(params SocketUser[] users)
            => base.WithUsers(users) as StaticPaginatorBuilder;

        /// <summary>
        /// Sets the users who can interact with the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithUsers(IEnumerable<SocketUser> users)
            => base.WithUsers(users) as StaticPaginatorBuilder;

        /// <summary>
        /// Sets the emotes and their related action of the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithEmotes(Dictionary<IEmote, PaginatorAction> emotes)
            => base.WithEmotes(emotes) as StaticPaginatorBuilder;

        /// <summary>
        /// Adds an emote related to a action to the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder AddEmote(PaginatorAction action, IEmote emote)
            => base.AddEmote(action, emote) as StaticPaginatorBuilder;

        /// <summary>
        /// Sets the <see cref="Embed"/> which the <see cref="Paginator"/> gets modified to after cancellation.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithCancelledEmbed(EmbedBuilder embed)
            => base.WithCancelledEmbed(embed) as StaticPaginatorBuilder;

        /// <summary>
        /// Sets the <see cref="Embed"/> which the <see cref="Paginator"/> gets modified to after a timeout.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithTimoutedEmbed(EmbedBuilder embed)
            => base.WithTimoutedEmbed(embed) as StaticPaginatorBuilder;

        /// <summary>
        /// Sets what the <see cref="Paginator"/> should delete.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithDeletion(DeletionOptions deletion)
            => base.WithDeletion(deletion) as StaticPaginatorBuilder;

        /// <summary>
        /// Sets the footer in the <see cref="Embed"/> of the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithFooter(PaginatorFooter footer)
            => base.WithFooter(footer) as StaticPaginatorBuilder;

        /// <summary>
        /// Clears all existing Emote-Action-Pairs and adds the standard Emote-Action-Pairs to the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new StaticPaginatorBuilder WithDefaultEmotes()
            => base.WithDefaultEmotes() as StaticPaginatorBuilder;

        /// <summary>
        /// Sets the pages of the <see cref="StaticPaginator"/>.
        /// </summary>
        /// <returns></returns>
        public StaticPaginatorBuilder WithPages(params PageBuilder[] pages)
        {
            Pages = pages.ToList();
            return this;
        }

        /// <summary>
        /// Sets the pages of the <see cref="StaticPaginator"/>.
        /// </summary>
        /// <returns></returns>
        public StaticPaginatorBuilder WithPages(IEnumerable<PageBuilder> pages)
        {
            Pages = pages.ToList();
            return this;
        }

        /// <summary>
        /// Adds a page to the <see cref="StaticPaginator"/>.
        /// </summary>
        /// <returns></returns>
        public StaticPaginatorBuilder AddPage(PageBuilder page)
        {
            Pages.Add(page);
            return this;
        }

        #endregion WithValue
    }
}