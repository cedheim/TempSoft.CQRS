using System;
using TempSoft.CQRS.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Commands;
using TempSoft.CQRS.Demo.Domain.Movies.Enums;
using TempSoft.CQRS.Demo.Domain.Movies.Events;
using TempSoft.CQRS.Demo.Domain.Movies.Exceptions;
using TempSoft.CQRS.Domain;
using TempSoft.CQRS.Events;

namespace TempSoft.CQRS.Demo.Domain.Movies.Entity
{
    public class Version : AggregateRoot<Movie>.Entity<Version>
    {
        public Version(Movie root, Guid id, string name) : base(root, id)
        {
            Name = name;
        }

        public string Name { get; }

        public bool Has3D { get; private set; }

        public bool HasIMAX { get; private set; }

        public bool HasTHX { get; private set; }

        [CommandHandler(typeof(AddMovieProperty))]
        public void AddMovieProperty(MovieProperties movieProperty)
        {
            switch (movieProperty)
            {
                case MovieProperties.Has3D:
                    if (Has3D)
                        throw new MoviePropertyException($"Trying to add duplicate movie property {movieProperty}");
                    break;
                case MovieProperties.HasIMAX:
                    if (HasIMAX)
                        throw new MoviePropertyException($"Trying to add duplicate movie property {movieProperty}");
                    break;
                case MovieProperties.HasTHX:
                    if (HasTHX)
                        throw new MoviePropertyException($"Trying to add duplicate movie property {movieProperty}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Root.ApplyChange(new AddedMovieProperty(Id, movieProperty));
        }

        [CommandHandler(typeof(RemoveMovieProperty))]
        public void RemoveMovieProperty(MovieProperties movieProperty)
        {
            switch (movieProperty)
            {
                case MovieProperties.Has3D:
                    if (!Has3D)
                        throw new MoviePropertyException($"Trying to remove movie property {movieProperty}");
                    break;
                case MovieProperties.HasIMAX:
                    if (!HasIMAX)
                        throw new MoviePropertyException($"Trying to remove movie property {movieProperty}");
                    break;
                case MovieProperties.HasTHX:
                    if (!HasTHX)
                        throw new MoviePropertyException($"Trying to remove movie property {movieProperty}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Root.ApplyChange(new RemovedMovieProperty(Id, movieProperty));
        }

        [EventHandler(typeof(AddedMovieProperty))]
        public void AddedMovieProperty(AddedMovieProperty @event)
        {
            switch (@event.MovieProperty)
            {
                case MovieProperties.Has3D:
                    Has3D = true;
                    break;
                case MovieProperties.HasIMAX:
                    HasIMAX = true;
                    break;
                case MovieProperties.HasTHX:
                    HasTHX = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        [EventHandler(typeof(RemovedMovieProperty))]
        public void RemovedMovieProperty(RemovedMovieProperty @event)
        {
            switch (@event.MovieProperty)
            {
                case MovieProperties.Has3D:
                    Has3D = false;
                    break;
                case MovieProperties.HasIMAX:
                    HasIMAX = false;
                    break;
                case MovieProperties.HasTHX:
                    HasTHX = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}