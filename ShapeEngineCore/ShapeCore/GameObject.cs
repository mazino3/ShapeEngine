using System.Numerics;
using Raylib_CsLo;

namespace ShapeCore
{
    /*public interface IGameObject
    {   
        public void Spawn();
        public void Draw();
        public void Update(float dt);
        public void Destroy();
        public bool IsDead();
        public float Damage(float amount, Vector2 pos, IGameObject dealer);
        public float GetDamage();


    }*/
    public class GameObject
    {
        public float DrawOrder { get; set; } = 0;
        public string Group { get; set; } = "default";
        public string AreaLayerName { get; set; } = "default";
        public Vector2 AreaLayerOffset { get; set; } = new(0f);

        /// <summary>
        /// Slows down or speeds up the gameobject. 2 means twice as fast, 0.5 means half speed. Is affect by slow resistance and area slow factor.
        /// </summary>
        public float UpdateSlowFactor { get; set; } = 1f;
        /// <summary>
        /// Multiplier for the total slow factor. 2 means slow factor has twice the effect, 0.5 means half the effect.
        /// </summary>
        public float UpdateSlowResistance { get; set; } = 1f;
        public GameObject() { }

        public bool IsInGroup(string group) { return this.Group == group; }


        public virtual void Start() { }
        public virtual void Destroy() { }
        public virtual void Draw() { }
        public virtual void DrawUI(Vector2 uiSize, Vector2 stretchFactor) { }
        public virtual void Update(float dt) { }
        public virtual void OnPlayfield(bool inner, bool outer) { }
        //public virtual float Damage(float amount, Vector2 pos, GameObject dealer) { return 0; }
        //public virtual float GetDamage() { return 0; }

        public virtual Rectangle GetBoundingBox() { return new(GetPosition().X, GetPosition().Y, 1, 1); }
        public virtual Vector2 GetPosition() { return Vector2.Zero; }
        public virtual Vector2 GetCameraPosition(Vector2 camPos, float dt, float smoothness = 1f, float boundary = 0f) { return GetPosition(); }
        public virtual bool IsDead() { return false; }
        public virtual bool Kill() { return false; }
        protected virtual void WasKilled() { }
        public virtual bool IsEnabled() { return true; }
        public virtual bool IsVisible() { return true; }

        //public virtual void MonitorHasChanged()
        //{
        //
        //}
    }
    /*public class GameObject
    {
        protected Vector2 pos = new(0, 0);
        protected Vector2 size = new(0, 0);
        protected bool dead = false;
        protected int drawOrder = 0;
        protected bool visible = false;
        protected bool enabled = false;
        protected string group = "NONE";

        public GameObject() { }



        public int GetDrawOrder() { return drawOrder; }
        public void SetDrawOrder(int drawOrder) { this.drawOrder = drawOrder; }
        public bool IsDead() { return dead; }
        
        public string GetGroup() { return group; }
        public void SetGroup(string group) { this.group = group; }
        public bool IsInGroup(string group) { return this.group == group; }
        
        public virtual void Spawn(int x, int y)
        {
            pos.X = x;
            pos.Y = y;
            dead = false;
        }
        public virtual void Destroy() { }
        public virtual void Draw() { }
        public virtual void Update(float dt) { }

        public virtual bool IsEnabled() { return enabled; }
        public virtual void Enable()
        {
            if (enabled) return;
            enabled = true;
        }
        public virtual void Disable()
        {
            if(!enabled) return;
            enabled = false;
        }

        public virtual bool IsVisible() { return visible; }
        public virtual void Show()
        {
            if (visible) return;
            visible = true;
        }
        public virtual void Hide()
        {
            if(!visible) return;
            visible = false;
        }


        public bool IsOnScreen(Rectangle screen)
        {
            return CheckCollisionRecs(GetBoundingBox(), screen);
        }
        public virtual Rectangle GetBoundingBox()
        {
            return new(pos.X - size.X * 0.5f, pos.Y - size.Y * 0.5f, size.X, size.Y);
        }
    }*/

}
