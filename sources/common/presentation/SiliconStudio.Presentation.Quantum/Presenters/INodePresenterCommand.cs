// Copyright (c) 2011-2017 Silicon Studio Corp. All rights reserved. (https://www.siliconstudio.co.jp)
// See LICENSE.md for full license information.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiliconStudio.Core.Annotations;

namespace SiliconStudio.Presentation.Quantum.Presenters
{
    public interface INodePresenterCommand
    {
        [NotNull]
        string Name { get; }

        CombineMode CombineMode { get; }

        bool CanAttach([NotNull] INodePresenter nodePresenter);

        bool CanExecute([NotNull] IReadOnlyCollection<INodePresenter> nodePresenters, object parameter);

        Task<object> PreExecute([NotNull] IReadOnlyCollection<INodePresenter> nodePresenters, object parameter);

        Task Execute([NotNull] INodePresenter nodePresenter, object parameter, object preExecuteResult);

        Task PostExecute([NotNull] IReadOnlyCollection<INodePresenter> nodePresenters, object parameter);
    }
}
