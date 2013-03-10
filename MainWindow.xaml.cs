using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;

namespace TangibleAnchoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        private const string submissionDataFile = "SubmissionData.xml";
        private readonly Submissions.SubmissionData submissionData;
        
        //WA = Work Area
        
        private double WAWidth = System.Windows.SystemParameters.WorkArea.Width;
        private double WAHeight = System.Windows.SystemParameters.WorkArea.Height;
        double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        private double leftMargin = 100;
        private double bottomMargin = 50;
        private double rightMargin = 50;
        private double topMargin = 50;

        public MainWindow()
        {
            InitializeComponent();
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            // Plug our item data into our visualizations
            submissionData = new Submissions.SubmissionData(submissionDataFile);
            DrawAxes();
            DrawPoints(submissionData);
        }

        private void LogMsg(string message)
        {
            LogMessageLabel.Content = message;
        }

        private void DrawAxes()
        {
            
            string message = "workarea: width = " + screenWidth + " height = " + screenHeight;
            LogMsg(message);
            Line xAxis = new Line(), yAxis = new Line();
            xAxis.Stroke = System.Windows.Media.Brushes.Beige;
            xAxis.X1 = leftMargin/2;
            xAxis.Y1 = WAHeight - bottomMargin/2;
            xAxis.X2 = WAWidth - rightMargin;//1900;
            xAxis.Y2 = xAxis.Y1;
            //xAxis.HorizontalAlignment = HorizontalAlignment.Left;
            //xAxis.VerticalAlignment = VerticalAlignment.Center;
            xAxis.StrokeThickness = 2;
            MainCanvas.Children.Add(xAxis);

            yAxis.Stroke = System.Windows.Media.Brushes.Aqua;
            yAxis.X1 = leftMargin / 2;
            yAxis.Y1 = WAHeight - bottomMargin / 2;
            yAxis.X2 = yAxis.X1;
            yAxis.Y2 = bottomMargin + topMargin;
            //yAxis.HorizontalAlignment = HorizontalAlignment.Left;
            //yAxis.VerticalAlignment = VerticalAlignment.Center;
            yAxis.StrokeThickness = 2;
            MainCanvas.Children.Add(yAxis);    

        }

        private void DrawPoints(Submissions.SubmissionData submissionData)
        {
            if (submissionData != null)
            {
                int numPoints = submissionData.Submissions.Length;
                
                
                SolidColorBrush[] viewpointColors = {
                                           SurfaceColors.Accent1Brush,
                                           SurfaceColors.Accent2Brush, 
                                           SurfaceColors.Accent3Brush, 
                                           SurfaceColors.Accent4Brush,
                                           SurfaceColors.BulletBrush, 
                                           SurfaceColors.ButtonBackgroundBrush, 
                                           SurfaceColors.ButtonBackgroundPressedBrush
                                       };

                for (int index = 0; index < numPoints; index++)
                {
                    Ellipse dataPoint = new Ellipse();
                    Submissions.Submission sData = submissionData.Submissions[index];
                    //TODO replace 700 by minimum value for the variable
                    double leftPosition = (double)(((int.Parse(sData.UserId) - 700)*2)%WAWidth);
                    double bottomPosition = (double)(((sData.Age)*10)%WAHeight);
                    dataPoint.SetValue(Canvas.LeftProperty, leftPosition);
                    dataPoint.SetValue(Canvas.BottomProperty, bottomPosition);
                    dataPoint.Height = 20;
                    dataPoint.Width = 20;

                    //double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
                    //double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
                    //string message = "Screen: Width = " + screenWidth + " Height = " + screenHeight;
                    //message += "\n WorkArea: Width = " + workArea.Width + " Height = " + workArea.Height;
                    //LogMsg(message);

                    // AnswerId - 1 because viewpoints are numbered from 1..7 
                    dataPoint.Fill = viewpointColors[int.Parse(sData.Responses[0].AnswerId) - 1];
                    MainCanvas.Children.Add(dataPoint);
                }
            }
        }

        /// <summary>
        /// Adds handlers for window availability events.
        /// </summary>
        private void AddWindowAvailabilityHandlers()
        {
            // Subscribe to surface window availability events
            ApplicationServices.WindowInteractive += OnWindowInteractive;
            ApplicationServices.WindowNoninteractive += OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable += OnWindowUnavailable;
        }

        /// <summary>
        /// Removes handlers for window availability events.
        /// </summary>
        private void RemoveWindowAvailabilityHandlers()
        {
            // Unsubscribe from surface window availability events
            ApplicationServices.WindowInteractive -= OnWindowInteractive;
            ApplicationServices.WindowNoninteractive -= OnWindowNoninteractive;
            ApplicationServices.WindowUnavailable -= OnWindowUnavailable;
        }

        /// <summary>
        /// This is called when the user can interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowInteractive(object sender, EventArgs e)
        {
            //TODO: enable audio, animations here
        }

        /// <summary>
        /// This is called when the user can see but not interact with the application's window.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowNoninteractive(object sender, EventArgs e)
        {
            //TODO: Disable audio here if it is enabled

            //TODO: optionally enable animations here
        }

        /// <summary>
        /// This is called when the application's window is not visible or interactive.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnWindowUnavailable(object sender, EventArgs e)
        {
            //TODO: disable audio, animations here
        }
    }
}
