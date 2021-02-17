using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Interactivity.Extensions;

namespace Interactivity.Pagination
{
    /// <summary>
    /// Represents a builder class for making a <see cref="LazyPaginator"/>.
    /// </summary>
    public sealed class LazyPaginatorBuilder : PaginatorBuilder
    {
        #region Fields

        /// <summary>
        /// Gets or sets the method used to load the pages of the <see cref="Paginator"/> lazily.
        /// </summary>
        public Func<int, Task<PageBuilder>> PageFactory { get; set; }

        /// <summary>
        /// Gets or sets the maximum page of the <see cref="Paginator"/>.
        /// </summary>
        public int MaxPageIndex { get; set; }

        #endregion Fields

        #region Build

        public override Paginator Build(int startPage = 0)
        {
            if (Emotes.Count == 0)
            {
                WithDefaultEmotes();
            }

            return new LazyPaginator(Users?.AsReadOnlyCollection() ?? throw new ArgumentNullException(nameof(Users)),
                                     Emotes?.AsReadOnlyDictionary() ?? throw new ArgumentNullException(nameof(Emotes)),
                                     CancelledEmbed?.Build(),
                                     TimeoutedEmbed?.Build(),
                                     Deletion,
                                     AddPaginatorFooterAsync,
                                     startPage,
                                     MaxPageIndex);

            async Task<Page> AddPaginatorFooterAsync(int page)
            {
                var builder = await PageFactory.Invoke(page).ConfigureAwait(false);

                return builder?.WithPaginatorFooter(Footer, page, MaxPageIndex, Users)
                               .Build();
            }
        }

        #endregion Build

        #region WithValue

        /// <summary>
        /// Sets the users who can interact with the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithUsers(params SocketUser[] users)
            => base.WithUsers(users) as LazyPaginatorBuilder;

        /// <summary>
        /// Sets the users who can interact with the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithUsers(IEnumerable<SocketUser> users)
            => base.WithUsers(users) as LazyPaginatorBuilder;

        /// <summary>
        /// Sets the emotes and their related action of the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithEmotes(Dictionary<IEmote, PaginatorAction> emotes)
            => base.WithEmotes(emotes) as LazyPaginatorBuilder;

        /// <summary>
        /// Adds an emote related to a action to the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder AddEmote(PaginatorAction action, IEmote emote)
            => base.AddEmote(action, emote) as LazyPaginatorBuilder;

        /// <summary>
        /// Sets the <see cref="Embed"/> which the <see cref="Paginator"/> gets modified to after cancellation.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithCancelledEmbed(EmbedBuilder embed)
            => base.WithCancelledEmbed(embed) as LazyPaginatorBuilder;

        /// <summary>
        /// Sets the <see cref="Embed"/> which the <see cref="Paginator"/> gets modified to after a timeout.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithTimoutedEmbed(EmbedBuilder embed)
            => base.WithTimoutedEmbed(embed) as LazyPaginatorBuilder;

        /// <summary>
        /// Sets what the <see cref="Paginator"/> should delete.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithDeletion(DeletionOptions deletion)
            => base.WithDeletion(deletion) as LazyPaginatorBuilder;

        /// <summary>
        /// Sets the footer in the <see cref="Embed"/> of the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithFooter(PaginatorFooter footer)
            => base.WithFooter(footer) as LazyPaginatorBuilder;

        /// <summary>
        /// Clears all existing Emote-Action-Pairs and adds the standard Emote-Action-Pairs to the <see cref="Paginator"/>.
        /// </summary>
        /// <returns></returns>
        public new LazyPaginatorBuilder WithDefaultEmotes()
            => base.WithDefaultEmotes() as LazyPaginatorBuilder;

        /// <summary>
        /// Sets the PageFactory of the <see cref="LazyPaginator"/>.
        /// </summary>
        /// <returns></returns>
        public LazyPaginatorBuilder WithPageFactory(Func<int, Task<PageBuilder>> pagefactory)
        {
            PageFactory = pagefactory;
            return this;
        }

        /// <summary>
        /// Sets the maximum page index of the <see cref="LazyPaginator"/>.
        /// </summary>
        /// <returns></returns>
        public LazyPaginatorBuilder WithMaxPageIndex(int maxPageIndex)
        {
            MaxPageIndex = maxPageIndex;
            return this;
        }

        #endregion WithValue
    }
}