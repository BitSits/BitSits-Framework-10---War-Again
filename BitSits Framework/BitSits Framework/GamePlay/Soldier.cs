using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;

namespace BitSits_Framework
{
    public enum Shape { Square, Triangle }

    public enum Weapon { Bazooka, AirBomb, LandMines, BigBully, ShortGun, BowArrow, Hammer, }

    class Soldier
    {
        GameContent gameContent;
        World world;

        List<Vector2> bullets;

        Animation walk, idle;
        AnimationPlayer animationPlayer;
        SpriteEffects spriteEffect = SpriteEffects.None;

        float showDirection, moveDirection; // Left = -1, Right = 1, Jump Up = 2
        float theta = (float)Math.PI / 2;

        const float MaxRemoveTime = 1.5f;
        float MaxHealth, MaxReloadTime, MaxJumpTime = .2f;
        float health, reloadTime, removeTime, jumpTime;

        const float Size = 25;
        Body body;
        Joint joint;

        bool isStable = false;

        public Soldier(Shape shape, Vector2 position, GameContent gameContent, World world)
        {
            this.gameContent = gameContent;
            this.world = world;

            idle = new Animation(gameContent.idle[(int)shape], 20f, false);
            walk = new Animation(gameContent.walk[(int)shape], 0.15f, true);
            animationPlayer.PlayAnimation(idle);

            MaxHealth = 10; health = gameContent.random.Next(2, 10);
            MaxReloadTime = gameContent.random.Next(50); reloadTime = 0;

            CircleShape cShape = new CircleShape();
            cShape._radius = (Size + 2) / 2 / gameContent.b2Scale;

            BodyDef bd = new BodyDef();
            bd.fixedRotation = true;
            bd.type = BodyType.Dynamic;
            bd.position = position / gameContent.b2Scale;
            body = world.CreateBody(bd);
            //body.SetLinearDamping(10);

            FixtureDef fd = new FixtureDef();
            fd.shape = cShape;
            fd.restitution = 0.5f;
            fd.friction = .1f;
            fd.density = .1f;

            body.CreateFixture(fd);
            body.SetUserData(this);
        }

        public Rectangle BoundingRect
        {
            get
            {
                int rectSize = 35;
                return new Rectangle((int)(body.Position.X * gameContent.b2Scale) - rectSize / 2,
                    (int)(body.Position.Y * gameContent.b2Scale) - rectSize / 2, rectSize, rectSize);
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (this != Level.selectedSoldier) { showDirection = 0; moveDirection = 0; }

            if (jumpTime > 0)
            {
                jumpTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                DestroyJoint();
                float d = theta - (float)Math.PI;
                body.ApplyLinearImpulse(new Vector2((float)Math.Cos(d), (float)Math.Sin(d)) * .015f, body.Position);
            }
            else
            {
                for (ContactEdge ce = body.GetContactList(); ce != null; ce = ce.Next)
                {
                    if (ce.Contact.IsTouching())
                    {
                        WorldManifold wm;
                        ce.Contact.GetWorldManifold(out wm);
                        Vector2 contact = new Vector2(wm._points[0].X, wm._points[0].Y);

                        float contactAngle = (float)Math.Atan2(contact.Y - body.Position.Y, contact.X - body.Position.X);

                        // no Joint OR preffer anything when unstable
                        if (joint == null || (moveDirection == 0 && !isStable))
                        {
                            CreateJoint(ce.Other, contact);
                        }
                        else if (body.GetJointList().Next == null && (moveDirection == 1 || moveDirection == -1))
                        {
                            float jointAngle = (float)Math.Atan2(joint.GetAnchorB().Y - joint.GetAnchorA().Y,
                                    joint.GetAnchorB().X - joint.GetAnchorA().X);

                            if (moveDirection > 0 && contactAngle > jointAngle) contactAngle -= 2 * (float)Math.PI;
                            if (moveDirection < 0 && contactAngle < jointAngle) contactAngle += 2 * (float)Math.PI;

                            if (Math.Abs(contactAngle - jointAngle) < Math.PI)
                                CreateJoint(ce.Other, contact);
                        }
                    }
                }
            }

            if (joint != null && joint.GetReactionForce(1).Length() > 1f) ;

            if (joint != null)
            {
                theta = (float)Math.Atan2(joint.GetAnchorB().Y - joint.GetAnchorA().Y,
                    joint.GetAnchorB().X - joint.GetAnchorA().X);

                if ((moveDirection == 1 || moveDirection == -1))
                {
                    if (isStable && body.GetJointList().Next == null) // only one joint
                    {
                        float d = theta - (float)Math.PI / 2 * moveDirection;
                        body.SetLinearVelocity(new Vector2((float)Math.Cos(d), (float)Math.Sin(d)) * 3f);
                    }
                    else Level.selectedSoldier = null; //moveDirection = 0;
                }
            }
            else isStable = false;

            if (body.GetLinearVelocity().Length() > 0.1f && (joint == null || moveDirection == 1 || moveDirection == -1))
                animationPlayer.PlayAnimation(walk);
            else animationPlayer.PlayAnimation(idle);

            if (reloadTime < MaxReloadTime)
                reloadTime = Math.Min(reloadTime + (float)gameTime.ElapsedGameTime.TotalSeconds * .9f, MaxReloadTime);
        }

        void DestroyJoint()
        {
            if (joint != null)
            {
                body.GetWorld().DestroyJoint(joint);
                joint = null;
            }
        }

        void CreateJoint(Body other, Vector2 contact)
        {
            // cannot move from stable situation to unstable
            if (isStable && other.GetUserData() is Soldier && !((Soldier)other.GetUserData()).isStable)
                return;

            DestroyJoint();

            if (!(other.GetUserData() is Soldier) || ((Soldier)other.GetUserData()).joint == null 
                || ((Soldier)other.GetUserData()).joint.GetBodyB() != body)
            {
                DistanceJointDef jd = new DistanceJointDef();

                //jd.frequencyHz = 10; jd.dampingRatio = 0f;
                jd.Initialize(body, other, body.Position, contact);
                jd.length = Size / 2 / gameContent.b2Scale;
                jd.collideConnected = true;
                joint = body.GetWorld().CreateJoint(jd);

                if (joint.GetBodyB().GetUserData() is Soldier)
                    isStable = ((Soldier)joint.GetBodyB().GetUserData()).isStable;
                else isStable = true;
            }
        }

        public void ShowMove(Vector2 mousePos)
        {
            showDirection = 0;
            if (BoundingRect.Contains(new Point((int)mousePos.X, (int)mousePos.Y))) return;

            if (isStable && joint != null && body.GetJointList().Next == null)
            {
                float d = (float)Math.Atan2(mousePos.Y - body.Position.Y * gameContent.b2Scale,
                    mousePos.X - body.Position.X * gameContent.b2Scale);

                d = (d - theta) / (float)Math.PI * 180;
                d = d <= -180 ? d + 360 : d >= 180 ? d - 360 : d;

                if (Math.Abs(d) > 150) showDirection = 2; // Jump up
                else if (d > 0) showDirection = -1; // Move Left
                else showDirection = 1; // Move Right
            }
        }

        public void Move(Vector2 mousePos)
        {
            if (showDirection == 0) Level.selectedSoldier = null;

            moveDirection = showDirection; showDirection = 0;

            if (moveDirection == 2)
            {
                Level.selectedSoldier = null; //moveDirection = 0;
                jumpTime = MaxJumpTime;
            }
        }

        public void DrawSelected(SpriteBatch spriteBatch)
        {
            Vector2 pos = body.Position * gameContent.b2Scale;

            if (moveDirection != 0 || showDirection != 0)
            {
                spriteBatch.Draw(gameContent.ring, pos - gameContent.ringOrigin, null, Color.White);

                if (showDirection != 0)
                    spriteBatch.Draw(gameContent.moveArrow, pos, null, Color.White, theta - (float)Math.PI / 2 * showDirection,
                        new Vector2(showDirection == 2 ? -26 : -22, gameContent.moveArrow.Height / 2), 1, SpriteEffects.None, 1);
            }
            else
                spriteBatch.Draw(gameContent.ringDashed, pos - gameContent.ringOrigin, null, Color.White);
        }

        public void DrawHealth(SpriteBatch spriteBatch)
        {
            Vector2 pos = body.Position * gameContent.b2Scale;

            if (showDirection == 0)//this != Level.selectedSoldier)
            {
                // health bar 
                int width = 20, height = 5;
                spriteBatch.Draw(gameContent.blank, new Rectangle((int)pos.X - width / 2, (int)pos.Y - 30, width, height),
                    Color.Gainsboro);
                spriteBatch.Draw(gameContent.blank, new Rectangle((int)pos.X - width / 2, (int)pos.Y - 30,
                    (int)(width * health / MaxHealth), height), Color.Crimson);

                height = 2;
                if (reloadTime < MaxReloadTime)
                {
                    spriteBatch.Draw(gameContent.blank, new Rectangle((int)pos.X - width / 2, (int)pos.Y - 23, width, height),
                        Color.Gainsboro);
                    spriteBatch.Draw(gameContent.blank, new Rectangle((int)pos.X - width / 2, (int)pos.Y - 23,
                        (int)(width * reloadTime / MaxReloadTime), height), Color.DimGray);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            if (moveDirection != 0)
                spriteEffect = moveDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            animationPlayer.Draw(gameTime, spriteBatch, body.Position * gameContent.b2Scale,
                Color.White, theta - (float)Math.PI / 2, spriteEffect);
        }
    }
}
