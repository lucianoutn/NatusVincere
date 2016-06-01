using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
//using Microsoft.DirectX.Direct3DX;
using TgcViewer;
using TgcViewer.Utils.Input;
using TgcViewer.Utils.TgcGeometry;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
using TgcViewer.Utils.TgcSceneLoader;



namespace AlumnoEjemplos.NatusVincere
{
    public class NVCamaraFps : TgcCamera
    {
        #region declaraciones
        

        public const float DEFAULT_ROTATION_SPEED = 2f;
        public const float DEFAULT_MOVEMENT_SPEED = 100f;
        public const float DEFAULT_JUMP_SPEED = 100f;
        public float accumPitchDegrees;
        public Vector3 eye;
        public Vector3 lookAt;
        public Vector3 xAxis;
        public Vector3 yAxis;
        public Vector3 zAxis;
        public Vector3 viewDir;
        readonly Vector3 WORLD_XAXIS = new Vector3(1.0f, 0.0f, 0.0f);
        readonly Vector3 WORLD_YAXIS = new Vector3(0.0f, 1.0f, 0.0f);
        readonly Vector3 WORLD_ZAXIS = new Vector3(0.0f, 0.0f, 1.0f);
        readonly Vector3 ARRIBA = new Vector3(0, 1, 0);
        readonly Vector3 DEFAULT_UP_VECTOR = new Vector3(0.0f, 1.0f, 0.0f);
        public Vector3 acceleration;
        public float rotationSpeed;
        public bool accelerationEnable;
        public Vector3 CAMERA_VELOCITY = new Vector3(DEFAULT_MOVEMENT_SPEED, DEFAULT_JUMP_SPEED, DEFAULT_MOVEMENT_SPEED);
        public Vector3 CAMERA_POS = new Vector3(0.0f, 1.0f, 0.0f);
        public Vector3 CAMERA_ACCELERATION = new Vector3(400f, 400f, 400f);
        public Vector3 currentVelocity;
        public Vector3 velocity;
        public TgcD3dInput.MouseButtons rotateMouseButton;
        bool moveForwardsPressed = false;
        bool moveBackwardsPressed = false;
        bool moveRightPressed = false;
        bool moveLeftPressed = false;
        bool moveUpPressed = false;
        bool moveDownPressed = false;
        TgcViewer.Utils.Logger log = GuiController.Instance.Logger;
        Microsoft.DirectX.Direct3D.Device d3dDevice = GuiController.Instance.D3dDevice;
        Human personaje;

        #endregion declaraciones
        

        #region capturar mouse

        public void captureMouse()
        {
            //log.log("entre a capmouse");
            int ScreenWidth = GuiController.Instance.D3dDevice.Viewport.Width;
            int ScreenHeight = GuiController.Instance.D3dDevice.Viewport.Height;
            int ScreenX = GuiController.Instance.D3dDevice.Viewport.X;
            int ScreenY = GuiController.Instance.D3dDevice.Viewport.Y;
            //log.log(ScreenX.ToString()+" "+ScreenY.ToString(), Color.Pink);

            Control focusWindows = d3dDevice.CreationParameters.FocusWindow;
            Cursor.Position = focusWindows.PointToScreen(new Point(focusWindows.Width / 2, focusWindows.Height / 2));
            
        }
         
        
        #endregion de capturar mouse


           
                    
        /// <summary>
        /// Crea la cámara con valores iniciales.
        /// Aceleración desactivada por Default
        /// </summary>
        public NVCamaraFps(Human personaje)
        {
            resetValues();
            this.personaje = personaje;
        }
        

        public void resetValues()
        {
            accumPitchDegrees = 0.0f;
            rotationSpeed = DEFAULT_ROTATION_SPEED;
            eye = new Vector3(0.0f, 0.0f, 0.0f);
            xAxis = new Vector3(1.0f, 0.0f, 0.0f);
            yAxis = new Vector3(0.0f, 1.0f, 0.0f);
            zAxis = new Vector3(0.0f, 0.0f, 1.0f);
            viewDir = new Vector3(0.0f, 0.0f, 1.0f);
            lookAt = eye + viewDir;

            accelerationEnable = false;
            acceleration = CAMERA_ACCELERATION;
            currentVelocity = new Vector3(0.0f, 0.0f, 0.0f);
            velocity = CAMERA_VELOCITY;
            viewMatrix = Matrix.Identity;
            CAMERA_POS.Y += alturaPreseteada;
            setPosition(CAMERA_POS);

            //rotateMouseButton = TgcD3dInput.MouseButtons.BUTTON_LEFT;
        }

        #region seteos
        bool enable;
        /// <summary>
        /// Habilita o no el uso de la camara
        /// </summary>
        public bool Enable
        {
            get { return enable; }
            set
            {
                enable = value;

                //Si se habilito la camara, cargar como la cámara actual
                if (value)
                {
                    GuiController.Instance.CurrentCamera = this;
                    //log.log("aca1", Color.Blue);
                    
                }
                else
                {
                    //GuiController.Instance.ThirdPersonCamera.Enable = true;
                }
            }
        }

        public float alturaPreseteada
        {
            get;
            set;
        }



        /*    public bool rotateCameraWithMouse //se pude fijar en true
            {
                get;
                set;
            }
        */

        public Vector3 Acceleration
        {
            get { return acceleration; }
            set { acceleration = value; }
        }

        public Vector3 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        
        public float RotationSpeed
        {
            get { return rotationSpeed; }
            set { rotationSpeed = value; }
        }

       public float JumpSpeed
        {
            get { return velocity.Y; }
            set { velocity.Y = value; }
        }

        public float MovementSpeed
        {
            get { return velocity.X; }
            set
            {
                velocity.X = value;
                velocity.Z = value;
            }
        }

        public Vector3 getPosition()
        {
            return eye;
        }

        public Vector3 getLookAt()
        {
            return lookAt;
        }


        public Vector3 LookAt
        {
            get { return lookAt; }
        }

        public bool AccelerationEnable
        {
            get { return accelerationEnable; }
            set { accelerationEnable = value; }
        }

        public Vector3 CurrentVelocity
        {
            get { return currentVelocity; }
        }

        public Vector3 Position
        {
            get { return eye; }
        }

        Matrix viewMatrix;
        /// <summary>
        /// View Matrix resultante
        /// </summary>
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
            set { viewMatrix = value; }
        }

        public void setCamera(Vector3 pos, Vector3 lookAt)
        {
            pos.Y += this.alturaPreseteada;
            lookAt.Y += alturaPreseteada;
            setCamera(pos, lookAt, DEFAULT_UP_VECTOR);
        }

        #endregion seteos

        public void reconstructViewMatrix(bool orthogonalizeAxes)
        {
            if (orthogonalizeAxes)
            {
                // Regenerate the camera's local axes to orthogonalize them.

                zAxis.Normalize();

                yAxis = Vector3.Cross(zAxis, xAxis);
                yAxis.Normalize();

                xAxis = Vector3.Cross(yAxis, zAxis);
                xAxis.Normalize();

                viewDir = zAxis;
                lookAt = eye + viewDir;
            }

            // Reconstruct the view matrix.

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);

            viewMatrix.M14 = 0.0f;
            viewMatrix.M24 = 0.0f;
            viewMatrix.M34 = 0.0f;
            viewMatrix.M44 = 1.0f;
        }

        public void updateViewMatrix(Microsoft.DirectX.Direct3D.Device d3dDevice)
        {
            if (!enable)
            {
                return;
            }

            d3dDevice.Transform.View = viewMatrix;
        }

        public void setCamera(Vector3 eye, Vector3 target, Vector3 up)
        {
            this.eye = eye;

            zAxis = target - eye;
            zAxis.Normalize();

            viewDir = zAxis;
            lookAt = eye + viewDir;

            xAxis = Vector3.Cross(up, zAxis);
            xAxis.Normalize();

            yAxis = Vector3.Cross(zAxis, xAxis);
            yAxis.Normalize();
            //xAxis.Normalize();

            viewMatrix = Matrix.Identity;

            viewMatrix.M11 = xAxis.X;
            viewMatrix.M21 = xAxis.Y;
            viewMatrix.M31 = xAxis.Z;
            viewMatrix.M41 = -Vector3.Dot(xAxis, eye);

            viewMatrix.M12 = yAxis.X;
            viewMatrix.M22 = yAxis.Y;
            viewMatrix.M32 = yAxis.Z;
            viewMatrix.M42 = -Vector3.Dot(yAxis, eye);

            viewMatrix.M13 = zAxis.X;
            viewMatrix.M23 = zAxis.Y;
            viewMatrix.M33 = zAxis.Z;
            viewMatrix.M43 = -Vector3.Dot(zAxis, eye);
            
            // Extract the pitch angle from the view matrix.
            accumPitchDegrees = Geometry.RadianToDegree((float)-Math.Asin((double)viewMatrix.M23));
           
        }



        public void setPosition(Vector3 pos)
        {
            //log.logVector3(pos, Color.Red);
            eye = pos;
            //personaje.setPosition(pos);
            //eye.Y += alturaPreset;
            //log.logVector3(eye, Color.Orange);
            reconstructViewMatrix(false);
        }


        public void updateCamera()
        {
            //Si la camara no está habilitada, no procesar el resto del input
            //log.log("update", Color.Red);
            if (!enable)
            {
                //log.log("update no enable", Color.Red);
                return;
            }

            float elapsedTimeSec = GuiController.Instance.ElapsedTime;
            TgcD3dInput d3dInput = GuiController.Instance.D3dInput;

            
            float heading = 0.0f;
            float pitch = 0.0f;

            //Obtener direccion segun entrada de teclado y mouse
            Vector3 direction = getMovementDirection(d3dInput);
            //log.logVector3(direction);
            captureMouse();
            pitch = d3dInput.YposRelative * rotationSpeed;
            //log.log(pitch.ToString());
            heading = d3dInput.XposRelative * rotationSpeed;
            //log.log(heading.ToString());

            //para que rote siempre
            rotateSmoothly(heading, pitch, 0.0f);

            /*
            Vector3[] values = this.game.tryToMovePlayer(this.Position, direction);
            Vector3 finalDirection = values[1];
            */

            //this.setPosition(values[0]);
            updatePosition(direction, elapsedTimeSec);
        }

        public Vector3 getMovementDirection(TgcD3dInput d3dInput)
        {
            Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);
           
            //Forward
            if (d3dInput.keyDown(Key.W))
            {
                if (!moveForwardsPressed)
                {
                    moveForwardsPressed = true;
                    currentVelocity = new Vector3(currentVelocity.X, currentVelocity.Y, 0.0f);
                    float elapsedTimeSec = GuiController.Instance.ElapsedTime;
                }

                direction.Z += 1.0f;
            }
            else
            {
                moveForwardsPressed = false;
            }

            //Backward
            if (d3dInput.keyDown(Key.S))
            {
                if (!moveBackwardsPressed)
                {
                    moveBackwardsPressed = true;
                    currentVelocity = new Vector3(currentVelocity.X, currentVelocity.Y, 0.0f);
                }

                direction.Z -= 1.0f;
            }
            else
            {
                moveBackwardsPressed = false;
            }

            //Strafe right
            if (d3dInput.keyDown(Key.D))
            {
                if (!moveRightPressed)
                {
                    moveRightPressed = true;
                    currentVelocity = new Vector3(0.0f, currentVelocity.Y, currentVelocity.Z);
                }

                direction.X += 1.0f;
            }
            else
            {
                moveRightPressed = false;
            }

            //Strafe left
            if (d3dInput.keyDown(Key.A))
            {
                if (!moveLeftPressed)
                {
                    moveLeftPressed = true;
                    currentVelocity = new Vector3(0.0f, currentVelocity.Y, currentVelocity.Z);
                }

                direction.X -= 1.0f;
            }
            else
            {
                moveLeftPressed = false;
            }

            //Jump
            if (d3dInput.keyDown(Key.Space))
            {
                if (!moveUpPressed)
                {
                    moveUpPressed = true;
                    currentVelocity = new Vector3(currentVelocity.X, 0.0f, currentVelocity.Z);
                }

                direction.Y += 1.0f;
            }
            else
            {
                moveUpPressed = false;
            }

            //Crouch
            if (d3dInput.keyDown(Key.LeftControl))
            {
                if (!moveDownPressed)
                {
                    moveDownPressed = true;
                    currentVelocity = new Vector3(currentVelocity.X, 0.0f, currentVelocity.Z);
                }

                direction.Y -= 1.0f;
            }
            else
            {
                moveDownPressed = false;
            }
            //personaje.movete(direction);
            return direction;
        }


        private void rotate(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            //log.log("entre a rotate");
            rollDegrees = -rollDegrees;
            rotateFirstPerson(headingDegrees, pitchDegrees);
            reconstructViewMatrix(true);
         }

        private void rotateSmoothly(float headingDegrees, float pitchDegrees, float rollDegrees)
        {
            headingDegrees *= rotationSpeed;
            pitchDegrees *= rotationSpeed;
            rollDegrees *= rotationSpeed;

            rotate(headingDegrees, pitchDegrees, rollDegrees);
        }

        private void updatePosition(Vector3 direction, float elapsedTimeSec)
        {
            //log.log("entre updatepos");
            //log.log(Vector3.LengthSq(currentVelocity).ToString());
            if (Vector3.LengthSq(currentVelocity) != 0.0f)
            {
                // Only move the camera if the velocity vector is not of zero length.
                // Doing this guards against the camera slowly creeping around due to
                // floating point rounding errors.

                Vector3 displacement;
                if (accelerationEnable)
                {
                    displacement = (currentVelocity * elapsedTimeSec) +
                    (0.5f * acceleration * elapsedTimeSec * elapsedTimeSec);
                }
                else
                {
                    displacement = (currentVelocity * elapsedTimeSec);
                }


                // Floating point rounding errors will slowly accumulate and cause the
                // camera to move along each axis. To prevent any unintended movement
                // the displacement vector is clamped to zero for each direction that
                // the camera isn't moving in. Note that the updateVelocity() method
                // will slowly decelerate the camera's velocity back to a stationary
                // state when the camera is no longer moving along that direction. To
                // account for this the camera's current velocity is also checked.

                if (direction.X == 0.0f && Math.Abs(currentVelocity.X) < 1e-6f)
                    displacement.X = 0.0f;

                if (direction.Y == 0.0f && Math.Abs(currentVelocity.Y) < 1e-6f)
                    displacement.Y = 0.0f;

                if (direction.Z == 0.0f && Math.Abs(currentVelocity.Z) < 1e-6f)
                    displacement.Z = 0.0f;

                move(displacement.X, displacement.Y, displacement.Z);
                //personaje.move(displacement);
            }

            // Continuously update the camera's velocity vector even if the camera
            // hasn't moved during this call. When the camera is no longer being moved
            // the camera is decelerating back to its stationary state.

            if (accelerationEnable)
            {
                updateVelocity(direction, elapsedTimeSec);
            }
            else
            {
                updateVelocityNoAcceleration(direction);
            }
        }

        private void rotateFirstPerson(float headingDegrees, float pitchDegrees)
        {
            //log.log("entre a rotate FPS");
            accumPitchDegrees += pitchDegrees;

            if (accumPitchDegrees > 90.0f)
            {
                pitchDegrees = 90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = 90.0f;
            }

            if (accumPitchDegrees < -90.0f)
            {
                pitchDegrees = -90.0f - (accumPitchDegrees - pitchDegrees);
                accumPitchDegrees = -90.0f;
            }

            float heading = Geometry.DegreeToRadian(headingDegrees);
            float pitch = Geometry.DegreeToRadian(pitchDegrees);

            Matrix rotMtx;
            Vector4 result;

            // Rotate camera's existing x and z axes about the world y axis.
            if (heading != 0.0f)
            {
                rotMtx = Matrix.RotationY(heading);

                result = Vector3.Transform(xAxis, rotMtx);
                xAxis = new Vector3(result.X, result.Y, result.Z);

                result = Vector3.Transform(zAxis, rotMtx);
                zAxis = new Vector3(result.X, result.Y, result.Z);

               // log.log(xAxis.ToString() + " " + zAxis.ToString(), Color.Blue);
            }

            // Rotate camera's existing y and z axes about its existing x axis.
            if (pitch != 0.0f)
            {
                rotMtx = Matrix.RotationAxis(xAxis, pitch);

                result = Vector3.Transform(yAxis, rotMtx);
                yAxis = new Vector3(result.X, result.Y, result.Z);

                result = Vector3.Transform(zAxis, rotMtx);
                zAxis = new Vector3(result.X, result.Y, result.Z);

               // log.log(yAxis.ToString() + " " + zAxis.ToString(), Color.Red);
            }
        }

        private void move(float dx, float dy, float dz)
        {

            //log.log("move");
            Vector3 auxEye = this.eye;
            Vector3 forwards;

            // Calculate the forwards direction. Can't just use the camera's local
            // z axis as doing so will cause the camera to move more slowly as the
            // camera's view approaches 90 degrees straight up and down.
            forwards = Vector3.Cross(xAxis, WORLD_YAXIS);
            forwards.Normalize();

            //log.logVector3(forwards, Color.Green);

            auxEye += xAxis * dx;
            auxEye += WORLD_YAXIS * dy;
            auxEye += forwards * dz;

            setPosition(auxEye);
            //personaje.movete(auxEye);
        }

        
        internal string getPositionCode()
        {
            //TODO ver de donde carajo sacar el LookAt de esta camara
            Vector3 lookAt = this.LookAt;
            
            return "GuiController.Instance.setCamera(new Vector3(" +
                TgcParserUtils.printFloat(eye.X) + "f, " + TgcParserUtils.printFloat(eye.Y) + "f, " + TgcParserUtils.printFloat(eye.Z) + "f), new Vector3(" +
                TgcParserUtils.printFloat(lookAt.X) + "f, " + TgcParserUtils.printFloat(lookAt.Y) + "f, " + TgcParserUtils.printFloat(lookAt.Z) + "f));";
        }

        private void move(Vector3 direction, Vector3 amount)
        {
            eye.X += direction.X * amount.X;
            eye.Y += direction.Y * amount.Y;
            eye.Z += direction.Z * amount.Z;

            reconstructViewMatrix(false);
        }

        private void updateVelocity(Vector3 direction, float elapsedTimeSec)
        {
            if (direction.X != 0.0f)
            {
                // Camera is moving along the x axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.X += direction.X * acceleration.X * elapsedTimeSec;

                if (currentVelocity.X > velocity.X)
                    currentVelocity.X = velocity.X;
                else if (currentVelocity.X < -velocity.X)
                    currentVelocity.X = -velocity.X;
            }
            else
            {
                // Camera is no longer moving along the x axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.X > 0.0f)
                {
                    if ((currentVelocity.X -= acceleration.X * elapsedTimeSec) < 0.0f)
                        currentVelocity.X = 0.0f;
                }
                else
                {
                    if ((currentVelocity.X += acceleration.X * elapsedTimeSec) > 0.0f)
                        currentVelocity.X = 0.0f;
                }
            }

            if (direction.Y != 0.0f)
            {
                // Camera is moving along the y axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.Y += direction.Y * acceleration.Y * elapsedTimeSec;

                if (currentVelocity.Y > velocity.Y)
                    currentVelocity.Y = velocity.Y;
                else if (currentVelocity.Y < -velocity.Y)
                    currentVelocity.Y = -velocity.Y;
            }
            else
            {
                // Camera is no longer moving along the y axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Y > 0.0f)
                {
                    if ((currentVelocity.Y -= acceleration.Y * elapsedTimeSec) < 0.0f)
                        currentVelocity.Y = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Y += acceleration.Y * elapsedTimeSec) > 0.0f)
                        currentVelocity.Y = 0.0f;
                }
            }

            if (direction.Z != 0.0f)
            {
                // Camera is moving along the z axis.
                // Linearly accelerate up to the camera's max speed.

                currentVelocity.Z += direction.Z * acceleration.Z * elapsedTimeSec;

                if (currentVelocity.Z > velocity.Z)
                    currentVelocity.Z = velocity.Z;
                else if (currentVelocity.Z < -velocity.Z)
                    currentVelocity.Z = -velocity.Z;
            }
            else
            {
                // Camera is no longer moving along the z axis.
                // Linearly decelerate back to stationary state.

                if (currentVelocity.Z > 0.0f)
                {
                    if ((currentVelocity.Z -= acceleration.Z * elapsedTimeSec) < 0.0f)
                        currentVelocity.Z = 0.0f;
                }
                else
                {
                    if ((currentVelocity.Z += acceleration.Z * elapsedTimeSec) > 0.0f)
                        currentVelocity.Z = 0.0f;
                }
            }
        }

        /// <summary>
        /// Actualizar currentVelocity sin aplicar aceleracion
        /// </summary>
        public void updateVelocityNoAcceleration(Vector3 direction)
        {
            currentVelocity.X = velocity.X * direction.X;
            currentVelocity.Y = velocity.Y * direction.Y;
            currentVelocity.Z = velocity.Z * direction.Z;
        }
                     
    }
}

