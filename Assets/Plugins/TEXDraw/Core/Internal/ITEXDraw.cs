using UnityEngine;


namespace TexDrawLib
{
    /// Interface that implemented for any TEXDraw component
    public interface ITEXDraw
    {
        /// Main string that used for rendering
        string text { get; set; }

        /// Initial text color that used for rendering
        Color color { get; set; }

        /// Initial document scale for rendering
        float size { get; set; }

        /// Local scaling amount for all characters
        Rect scrollArea { get; set; }

        /// Local scaling amount for all characters
        Vector2 alignment { get; set; }

        /// Local scaling amount for all characters
        TexRectOffset padding { get; set; }

        /// Access for internal Cached Preference
        TEXPreference preference { get; }

        /// Access to internal Orchestrator Engine
        TexOrchestrator orchestrator { get; }

        /// Will Trigger reparsing
        void SetTextDirty();
    }

    /// Interface that implemented for any TEXDraw renderer
    public interface ITexRenderer
    {
        /// Access to parent TEXDraw component
        ITEXDraw TEXDraw { get; }

        /// Get or set font responsible for rendering
        /// Note: some sentinel modes that used:
        /// -1: Dead, -2: Front Render, -3: Back Render
        int FontMode { get; set; }
    }
}
