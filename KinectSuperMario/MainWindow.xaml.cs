using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Kinect;


namespace KinectSuperMario
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ImageBrush textBrush = new ImageBrush();
            textBrush.ImageSource = new BitmapImage(new Uri("background.jpg", UriKind.Relative));
            //textBox.Background = textBrush;
            textBox.Text = ("Start!The first....");

            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = new BitmapImage(new Uri("1.png", UriKind.Relative));
            button.Background = imageBrush;
        }

        bool isWindowsClosing = false; //窗口是否正在关闭中
        Skeleton[] allSkeletons = new Skeleton[6];
        int count = 1;
        int cou_th = 0;


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            labelIsSkeletonTracked.Visibility = System.Windows.Visibility.Hidden;
            kinectSensorChooser1.KinectSensorChanged += new DependencyPropertyChangedEventHandler(kinectSensorChooser1_KinectSensorChanged);
        }

        void kinectSensorChooser1_KinectSensorChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            KinectSensor oldKinect = (KinectSensor)e.OldValue;

            stopKinect(oldKinect);

            KinectSensor kinect = (KinectSensor)e.NewValue;

            if (kinect == null)
            {
                return;
            }

            kinect.DepthStream.Enable(DepthImageFormat.Resolution640x480Fps30);
            kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
            var parameters = new TransformSmoothParameters
            {
                Smoothing = 0.5f,
                Correction = 0.5f,
                Prediction = 0.5f,
                JitterRadius = 0.05f,
                MaxDeviationRadius = 0.04f
            };
            kinect.SkeletonStream.Enable(parameters);

            kinect.SkeletonFrameReady += new EventHandler<SkeletonFrameReadyEventArgs>(kinect_SkeletonFrameReady);
            
            try
            {
                //显示彩色图像摄像头
                kinectColorViewer1.Kinect = kinect;

                //启动
                kinect.Start();
            }
            catch (System.IO.IOException)
            {
                kinectSensorChooser1.AppConflictOccurred();
            }
        }

        void kinect_SkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            //骨骼跟踪状态提示
            labelIsSkeletonTracked.Visibility = System.Windows.Visibility.Hidden;

            if (isWindowsClosing)
            {
                return;
            }

            //Get a skeleton
            Skeleton s = getClosetSkeleton(e);

            if (s == null)
            {
                return;
            }

            if (s.TrackingState != SkeletonTrackingState.Tracked)            {
                
                return;
            }

            //提示用户骨骼跟踪就绪,可以进行操作
            if (s.TrackingState == SkeletonTrackingState.Tracked)
                labelIsSkeletonTracked.Visibility = System.Windows.Visibility.Visible;

            

            //映射键盘事件,演示PPT
            present(s);
        }
       

        private bool isForwardGestureActive = false;
        private bool isBackGestureActive = false;
        private bool isBlackScreenActive = false;
        private bool isActive = false;

        private bool is_1 = false;
        private bool is_2 = false;
        private bool is_3 = false;
        private bool is_4 = false;
        private bool is_5 = false;
        private bool is_6 = false;
        private bool is_7 = false;

        private const double ArmStretchedThreadhold4PPT = 0.4; //演示PPT时，手臂水平伸展的阀值，单位米
        private const double ArmRaisedThreshhold = 0.1; //手臂垂直举起的阀值，单位米

        private void present(Skeleton s)
        {
            SkeletonPoint head = s.Joints[JointType.Head].Position;
            SkeletonPoint leftshoulder = s.Joints[JointType.ShoulderLeft].Position;
            SkeletonPoint rightshoulder = s.Joints[JointType.ShoulderRight].Position;

            SkeletonPoint leftHand = s.Joints[JointType.HandLeft].Position;
            SkeletonPoint rightHand = s.Joints[JointType.HandRight].Position;

            SkeletonPoint leftFoot = s.Joints[JointType.FootLeft].Position;
            SkeletonPoint rightFoot = s.Joints[JointType.FootRight].Position;

            SkeletonPoint rightElbow = s.Joints[JointType.ElbowRight].Position;
            SkeletonPoint leftElbow = s.Joints[JointType.ElbowLeft].Position;

            SkeletonPoint spine = s.Joints[JointType.Spine].Position;

            bool isRightHandRaised = (rightHand.Y - rightshoulder.Y) > ArmRaisedThreshhold;
            bool isLeftHandRaised = (leftHand.Y - leftshoulder.Y) > ArmRaisedThreshhold;

            bool isRightHandStretched = (rightHand.X - rightshoulder.X) > ArmStretchedThreadhold4PPT;
            bool isLeftHandStretched = (leftshoulder.X - leftHand.X) > ArmStretchedThreadhold4PPT;

            //动作1
            bool isX_1 = (rightHand.X - leftHand.X) <0.05;
            bool isY_1 = (rightHand.Y - leftHand.Y) < 0.05;
            bool isRightHand_1 = (rightshoulder.Y - rightHand.Y) > 0.3;
            bool isLeftHand_1= (leftshoulder.Y - leftHand.Y) > 0.3;

           
                    if (isX_1 && isY_1 && isRightHand_1 && isLeftHand_1)
                    {
                        if (!is_1 && !is_2 && !is_3 && !is_4 && !is_5 && !is_6 && !is_7)
                        {
                            is_1 = true;
                            err.Text = ("    1号动作通过！");

                            textBox.Text = ("2号动作");
                            //String url = count + ".png";
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = new BitmapImage(new Uri("2.png", UriKind.Relative));
                            button.Background = imageBrush;   
                            //System.Windows.Forms.SendKeys.SendWait("{1}");
                        }
                    }
                    else
                    {
                        is_1 = false;
                        //err.Text = ("动作1失败");
                    }


                    //动作2
                   bool isFoot_2 = (rightFoot.X - leftFoot.X) > 0.5;
                    if (isFoot_2)
                    {
                        if (!is_1 && !is_2 && !is_3 && !is_4 && !is_5 && !is_6 && !is_7)
                        {
                            is_2 = true;
                            err.Text = ("  2号动作通过！");
                            textBox.Text = ("3号动作");
                            //String url = count + ".png";
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = new BitmapImage(new Uri("3.png", UriKind.Relative));
                            button.Background = imageBrush;   
                           // System.Windows.Forms.SendKeys.SendWait("{2}");
                        }
                    }
                    else
                    {
                        is_2 = false;
                    }
                 //  break;


                    //动作3
             //  case 3:
                   //cou_th++;
                    //textBox.Text = ("3号动作" +cou_th);
                    bool isRightHand_3 = (rightHand.Y - rightshoulder.Y) > 0.4;
                    bool isLeftHand_3 = (leftHand.Y - leftshoulder.Y) > 0.4;
                     float tx=rightHand.Y - rightshoulder.Y;
                        float ty = leftHand.Y - leftshoulder.Y;
                    if (isRightHand_3)
                    {
                        if (!is_1 && !is_2 && !is_3 && !is_4 && !is_5 && !is_6 && !is_7)
                        {
                            is_3 = true;
                            err.Text = ("  3号动作通过！");
                            textBox.Text = ("4号动作");
                            //String url = count + ".png";
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = new BitmapImage(new Uri("4.png", UriKind.Relative));
                            button.Background = imageBrush;
                            // System.Windows.Forms.SendKeys.SendWait("{2}");  
                            //System.Windows.Forms.SendKeys.SendWait("{3}");
                        }
                    }
                    else
                    {
                        is_3 = false;
                       // err.Text = ("动作3失败"+tx+"/"+ty);
                    }
                 //  break;

                    //动作4
               // case 4:
                   //textBox.Text = ("......" + number);
                    bool isRightHand_4 = (rightshoulder.Y - rightHand.Y) > ArmRaisedThreshhold;
                    bool isLeftHand_4 = (leftHand.Y - leftshoulder.Y) > ArmRaisedThreshhold;
                    if (isRightHand_4 && isLeftHand_4)
                    {
                        if (!is_1 && !is_2 && !is_3 && !is_4 && !is_5 && !is_6 && !is_7)
                        {
                            is_4 = true;
                            err.Text = ("  4号动作通过！");
                            textBox.Text = ("5号动作");
                            //String url = count + ".png";
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = new BitmapImage(new Uri("5.png", UriKind.Relative));
                            button.Background = imageBrush;
                            // System.Windows.Forms.SendKeys.SendWait("{2}");
                            System.Windows.Forms.SendKeys.SendWait("{4}");
                        }
                    }
                    else
                    {
                        is_4 = false;
                    }
                  //  break;

                    //动作5
               // case 5:
                    bool isRightHand_5 = (rightHand.X - rightshoulder.X) > 0.4;
                    bool isLeftHand_5 = (leftHand.Y - leftshoulder.Y) > 0.4;
                    bool isRightHand_5_1 = (rightHand.Y - rightshoulder.Y) < 0.1;
                    if (isRightHand_5 && isLeftHand_5 && isRightHand_5_1)
                    {
                        if (!is_1 && !is_2 && !is_3 && !is_4 && !is_5 && !is_6 && !is_7)
                        {
                            err.Text = ("  5号动作通过！");
                            textBox.Text = ("6号动作");
                            //String url = count + ".png";
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = new BitmapImage(new Uri("6.png", UriKind.Relative));
                            button.Background = imageBrush;
                            // System.Windows.Forms.SendKeys.SendWait("{2}");
                            System.Windows.Forms.SendKeys.SendWait("{5}");
                        }
                    }
                    else
                    {
                        is_5 = false;
                    }
                //    break;

                    //动作6
             //   case 6:
                    bool isRightHand_6 = (rightHand.X - rightshoulder.X) > ArmStretchedThreadhold4PPT;
                    bool isLeftHand_6 = (leftshoulder.Y - leftHand.Y) > ArmRaisedThreshhold;

                    if (isRightHand_6 && isLeftHand_6)
                    {
                        if (!is_1 && !is_2 && !is_3 && !is_4 && !is_5 && !is_6 && !is_7)
                        {
                            is_6 = true;
                            err.Text = ("  6号动作通过！");
                            textBox.Text = ("7号动作");
                            //String url = count + ".png";
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = new BitmapImage(new Uri("7.png", UriKind.Relative));
                            button.Background = imageBrush;
                            // System.Windows.Forms.SendKeys.SendWait("{2}");
                            System.Windows.Forms.SendKeys.SendWait("{6}");
                        }
                    }
                    else
                    {
                        is_6 = false;
                    }
                //    break;

                    //动作7
              //  case 7:
                    bool isRightHand_7 = (rightHand.Y - rightshoulder.Y) < 0.1;
                    bool isLeftHand_7 = (leftHand.Y - leftshoulder.Y) < 0.1;
                    bool isRightHand_7_1 = (rightHand.X - rightshoulder.X) > 0.4;
                    bool isLeftHand_7_1 = (leftshoulder.X - leftHand.X) > 0.4;
                    if (isRightHand_7 && isLeftHand_7 && isRightHand_7_1 && isLeftHand_7_1)
                    {
                        if (!is_1 && !is_2 && !is_3 && !is_4 && !is_5 && !is_6 && !is_7)
                        {
                            is_7 = true;
                            err.Text = ("  7号动作通过！");
                            textBox.Text = ("1号动作");
                            //String url = count + ".png";
                            ImageBrush imageBrush = new ImageBrush();
                            imageBrush.ImageSource = new BitmapImage(new Uri("1.png", UriKind.Relative));
                            button.Background = imageBrush;
                            // System.Windows.Forms.SendKeys.SendWait("{2}");
                            System.Windows.Forms.SendKeys.SendWait("{7}");
                        }
                    }
                    else
                    {
                        is_7 = false;
                    }
               //     break;

              //  default:
              //      break;
         //   }
           /* //使用状态变量，避免多次重复发送键盘事件
            if (isRightHandStretched)
            {
                if (!isBackGestureActive && !isForwardGestureActive)
                {
                    isForwardGestureActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{A}");
                }
            }
            else
            {
                isForwardGestureActive = false;
            }

            if (isLeftHandStretched)
            {
                if (!isBackGestureActive && !isForwardGestureActive)
                {
                    isBackGestureActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{B}");
                }
            }
            else
            {
                isBackGestureActive = false;
            }

            ////左手举起，启动
            if (isLeftHandRaised )
            {
                if (!isActive)
                {
                    isActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{C}");
                }
            }
            else
            {
                isActive = false;
            }

            ////右手举起，屏幕变黑
            if (isRightHandRaised)
            {
                if (!isBlackScreenActive)
                {
                    isBlackScreenActive = true;
                    System.Windows.Forms.SendKeys.SendWait("{D}");
                }
            }
            else
            {
                isBlackScreenActive = false;
            }
              */  
        }
               
      

        Skeleton getClosetSkeleton(SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame skeletonFrameData = e.OpenSkeletonFrame())
            {
                if (skeletonFrameData == null)
                {
                    return null;
                }

                skeletonFrameData.CopySkeletonDataTo(allSkeletons);

                //Linq语法，查找离Kinect最近的、被跟踪的骨骼
                Skeleton closestSkeleton = (from s in allSkeletons
                                            where s.TrackingState == SkeletonTrackingState.Tracked &&
                                                  s.Joints[JointType.Head].TrackingState == JointTrackingState.Tracked
                                            select s).OrderBy(s => s.Joints[JointType.Head].Position.Z)
                                    .FirstOrDefault();

                return closestSkeleton;
            }
        }

        private void stopKinect(KinectSensor sensor)
        {
            if (sensor != null)
            {
                if (sensor.IsRunning)
                {
                    //stop sensor 
                    sensor.Stop();

                    //关闭音频流，如果当前已打开的话
                    if (sensor.AudioSource != null)
                    {
                        sensor.AudioSource.Stop();
                    }
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isWindowsClosing = true;
            stopKinect(kinectSensorChooser1.Kinect); 
        }



    }
}
