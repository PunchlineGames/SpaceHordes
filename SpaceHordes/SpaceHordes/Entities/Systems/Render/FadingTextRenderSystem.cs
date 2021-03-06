using GameLibrary.Dependencies.Entities;
using Microsoft.Xna.Framework.Graphics;
using SpaceHordes.Entities.Components;

namespace SpaceHordes.Entities.Systems.Render
{
    public class FadingTextRenderSystem : EntityProcessingSystem
    {
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        public FadingTextRenderSystem(SpriteBatch spriteBatch, SpriteFont spriteFont)
            : base(typeof(FadingText))
        {
            this.spriteBatch = spriteBatch;
            this.spriteFont = spriteFont;
        }

        public override void Process()
        {
            spriteBatch.Begin();

            base.Process();

            spriteBatch.End();
        }

        public override void Process(Entity e)
        {
            if (!e.HasComponent<FadingText>())
                return;

            e.GetComponent<FadingText>().Draw(spriteBatch, spriteFont);
        }
    }
}