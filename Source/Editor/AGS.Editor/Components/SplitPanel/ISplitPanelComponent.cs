﻿using AGS.API;

namespace AGS.Editor
{
    /// <summary>
    /// Adds the ability to place 2 panels adjacent to each other (either vertically or horizontally),
    /// and have a split line in between which can be dragged to resize both panels at the same time.
    /// </summary>
    public interface ISplitPanelComponent : IComponent
    {
        /// <summary>
        /// Gets or sets the top panel (or the left panel, if this is a horizontal split panel).
        /// </summary>
        /// <value>The top panel.</value>
        IPanel TopPanel { get; set; }

        /// <summary>
        /// Gets or sets the bottom panel (or the right panel, if this is a horizontal split panel).
        /// </summary>
        /// <value>The bottom panel.</value>
        IPanel BottomPanel { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:AGS.API.ISplitPanelComponent"/> is horizontal (or vertical- which is the default).
        /// </summary>
        /// <value><c>true</c> if is horizonal; otherwise, <c>false</c>.</value>
        bool IsHorizontal { get; set;}

        /// <summary>
        /// By default the split line is located at the bottom of the top panel (or at the right of the left panel).
        /// You can add an offset that will move and/or adjust the size of the split line.
        /// </summary>
        /// <value>The split line offset.</value>
        PointF SplitLineOffset { get; set; }

        /// <summary>
        /// Gets or sets the width of the visible split line.
        /// </summary>
        /// <value>The width of the line.</value>
        float SplitLineWidth { get; set; }
    }
}