﻿using System.Numerics;
using ShapeEngineCore.SimpleCollision;
using ShapeEngineDemo.Bodies;
using Raylib_CsLo;
using ShapeEngineCore.Globals;
using ShapeEngineCore.Globals.Persistent;
using ShapeEngineDemo.DataObjects;
using ShapeEngineCore.Globals.Audio;
using ShapeEngineCore;

namespace ShapeEngineDemo.Projectiles
{
    public struct ProjectileInfo
    {
        public Vector2 pos;
        public Vector2 dir;
        public Color color;
        public IDamageable dmgDealer;
        public float speedVariation;

        public ProjectileInfo(Vector2 pos, Vector2 dir, Color color, IDamageable dmgDealer, float speedVar = 0f)
        {
            this.pos = pos;
            this.dir = dir;
            this.color = color;
            this.dmgDealer = dmgDealer;
            this.speedVariation = speedVar;
        }
    }
    public class Projectile : GameObject, ICollidable
    {
        static readonly string[] collisionMask = new string[] { "asteroid" };

        protected string type = "";
        protected ColliderClass colliderClass = ColliderClass.AREA;
        protected CircleCollider collider;
        protected IDamageable dmgDealer;
        protected Color color;
        protected bool dead = false;
        protected float timer = 0f;
        public StatHandler stats = new(("dmg", 10f), ("critChance", 0f), ("critBonus", 2.5f), ("lifetime", 0f), ("speed", 0f));

        public Projectile(string type, ProjectileInfo info)
        {
            drawOrder = 15;
            this.type = type;
            this.dmgDealer = info.dmgDealer;
            Vector2 vel = new(0f, 0f);
            float size = 3f;
            var data = DataHandler.Get<ProjectileData>("projectiles", type);
            if(data != null)
            {
                stats.SetStat("dmg", data.dmg);
                stats.SetStat("critChance", data.critChance);
                stats.SetStat("critBonus", data.critBonus);
                stats.SetStat("lifetime", data.lifetime);
                stats.SetStat("speed", data.speed);
                this.timer = stats.Get("lifetime");
                this.color = info.color;
                float a = RNG.randF(-data.accuracy, data.accuracy);
                float speed = stats.Get("speed");
                float finalSpeed = speed + speed * RNG.randF(-info.speedVariation, info.speedVariation);
                vel = Vec.Rotate(Vec.Normalize(info.dir), a) * finalSpeed;
                size = data.size;
            }
            collider = new CircleCollider(info.pos, vel, size);
        }
        public Projectile(string type, ProjectileInfo info, Dictionary<string, StatSimple> bonuses)
        {
            drawOrder = 15;
            this.type = type;
            this.dmgDealer = info.dmgDealer;
            stats.SetBonuses(bonuses);
            Vector2 vel = new(0f, 0f);
            float size = 3f;
            var data = DataHandler.Get<ProjectileData>("projectiles", type);
            if (data != null)
            {
                stats.SetStat("dmg", data.dmg);
                stats.SetStat("critChance", data.critChance);
                stats.SetStat("critBonus", data.critBonus);
                stats.SetStat("lifetime", data.lifetime);
                stats.SetStat("speed", data.speed);
                this.timer = stats.Get("lifetime");
                this.color = info.color;
                float a = RNG.randF(-data.accuracy, data.accuracy);
                float speed = stats.Get("speed");
                float finalSpeed = speed + speed * RNG.randF(-info.speedVariation, info.speedVariation);
                vel = Vec.Rotate(Vec.Normalize(info.dir), a) * finalSpeed;
                size = data.size;
            }
            collider = new CircleCollider(info.pos, vel, size);
        }

        public Collider GetCollider() { return collider; }
        public ColliderClass GetColliderClass() { return this.colliderClass; }
        public ColliderType GetColliderType() { return ColliderType.DYNAMIC; }
        public string GetCollisionLayer() { return "bullet"; }
        public string[] GetCollisionMask() { return collisionMask; }
        public string GetID() { return type; }
        public override Vector2 GetPosition() { return collider.Pos; }
        public Vector2 GetPos() { return collider.Pos; }


        public virtual void Collide(CastInfo info) { }
        public virtual void Overlap(OverlapInfo info) { }
        public override bool IsDead() { return dead; }
        public override bool Kill()
        {
            if (dead) return false;
            dead = true;
            collider.Disable();
            WasKilled();
            return dead;
        }

        protected override void WasKilled()
        {
            SpawnDeathEffect();
        }
        public override void Update(float dt)
        {
            if (IsDead()) return;
            if (UpdateLifetimeTimer(dt)) return;
            stats.Update(dt);
            Move(dt);
            
            var info = GAMELOOP.GetCurArea().GetCurPlayfield().Collide(collider.Pos, collider.Radius);
            if (info.collided) PlayfieldCollision(info.hitPoint, info.n);
        }
        public override void Draw()
        {
            DrawCircle((int)collider.Pos.X, (int)collider.Pos.Y, collider.Radius, color);
        }

        protected DamageInfo ImpactDamage(IDamageable target)
        {
            float finalDamage = stats.Get("dmg");
            bool critted = false;
            float critChance = stats.Get("critChance");
            if(critChance > 0f)
            {
                if (RNG.randF() < critChance) { finalDamage *= stats.Get("critBonus"); critted = true; }
            }
            if (critted) AudioHandler.PlaySFX("projectile crit", -1f, -1f, 0.1f);
            return target.Damage(finalDamage, collider.Pos, Vec.Normalize(collider.Vel), dmgDealer, critted);
        }
        protected virtual void Move(float dt)
        {
            collider.ApplyAccumulatedForce(dt);
            collider.Pos += collider.Vel * dt;
        }
        protected virtual bool UpdateLifetimeTimer(float dt)
        {
            if (timer > 0f)
            {
                timer -= dt;
                if (timer <= 0f)
                {
                    timer = 0f;
                    Kill();
                    return true;
                }
            }
            return false;
        }
        protected virtual void PlayfieldCollision(Vector2 hitPos, Vector2 normal)
        {
            collider.Pos = hitPos;
            Kill();
        }
        protected virtual void SpawnDeathEffect()
        {
            AsteroidDeathEffect ade = new(collider.Pos, 0.25f, collider.Radius * 1.5f, color);
            GAMELOOP.AddGameObject(ade);
        }
    }
}