using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Text;
using Microsoft.Surface;
using Microsoft.Surface.Presentation;
using Microsoft.Surface.Presentation.Controls;
using Microsoft.Surface.Presentation.Input;
using System.Globalization;
using TangibleAnchoring.Submissions;

namespace TangibleAnchoring
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : SurfaceWindow
    {
        private const string submissionDataFile = "SubmissionData.xml";
        private readonly Submissions.SubmissionData submissionData;
        private Ellipse[] dataPointEllipses;

        //WA = Work Area

        private double WAWidth = System.Windows.SystemParameters.WorkArea.Width;
        private double WAHeight = System.Windows.SystemParameters.WorkArea.Height;
        double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
        double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
        private double leftMargin = 100;
        private double bottomMargin = 50;
        private double rightMargin = 50;
        private double topMargin = 50;
        Ellipse haloEllipse = new Ellipse();

        // The following distances specify how far the description textbox should be
        // from the center of the touch device.
        private const double nonTagDescriptionXDistance = 40.0;
        private const double tagDescriptionXDistance = 190.0;
        private const double descriptionYDistance = 30.0;

        public MainWindow()
        {
            InitializeComponent();
            // Add handlers for window availability events
            AddWindowAvailabilityHandlers();

            // Plug our item data into our visualizations
            submissionData = new Submissions.SubmissionData(submissionDataFile);
            dataPointEllipses = new Ellipse[submissionData.Submissions.Length];

            DrawAxes();
            DrawPoints(submissionData);
        }

        /// <summary>
        /// Shows a log message on the top left.
        /// </summary>
        /// <param name="message">string message</param>
        private void LogMsg(string message)
        {
            LogMessageLabel.Content = message;
        }

        /// <summary>
        /// Logic for drawing X, Y axes.
        /// </summary>
        private void DrawAxes()
        {

            string message = "workarea: width = " + screenWidth + " height = " + screenHeight;
            LogMsg(message);
            Line xAxis = new Line(), yAxis = new Line();
            xAxis.Stroke = System.Windows.Media.Brushes.Beige;
            xAxis.X1 = leftMargin / 2;
            xAxis.Y1 = WAHeight - bottomMargin / 2;
            xAxis.X2 = WAWidth - rightMargin;
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


        /// <summary>
        /// Logic for drawing data points.
        /// </summary>
        /// <param name="submissionData">collection of points</param>
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
                    dataPointEllipses[index] = new Ellipse();
                    Submissions.Submission sData = submissionData.Submissions[index];
                    //TODO replace 700 by minimum value for the variable
                    double leftPosition = (double)(((int.Parse(sData.UserId) - 700) * 2) % WAWidth);
                    double bottomPosition = (double)(((sData.Age) * 10) % WAHeight);
                    dataPointEllipses[index].SetValue(Canvas.LeftProperty, leftPosition);
                    dataPointEllipses[index].SetValue(Canvas.BottomProperty, bottomPosition);
                    dataPointEllipses[index].Name = "data_" + index;
                    //Dynamic assignment of touch event handler
                    dataPointEllipses[index].TouchEnter += new EventHandler<TouchEventArgs>(DataPointTouchEnter);
                    dataPointEllipses[index].TouchLeave += new EventHandler<TouchEventArgs>(DataPointTouchLeave);
                    dataPointEllipses[index].Height = 20;
                    dataPointEllipses[index].Width = 20;

                    // AnswerId - 1 because viewpoints are numbered from 1..7 
                    dataPointEllipses[index].Fill = viewpointColors[int.Parse(sData.Responses[0].AnswerId) - 1];
                    MainCanvas.Children.Add(dataPointEllipses[index]);
                }
            }
        }

         /// <summary>
        /// Touch Event handler for dynamically added data point.
        /// </summary>
        /// <param name="sender">element that invokes this handler (e.g an Ellipse)</param>
        /// <param name="e">touch event information such type of device</param>
        private void DataPointTouchLeave(object sender, TouchEventArgs e)
        {
            Ellipse senderEllipse = sender as Ellipse;
            MainCanvas.Children.Remove(haloEllipse);
            Description.Visibility = Visibility.Hidden;
            ConnectingLine.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Touch Event handler for dynamically added data point.
        /// </summary>
        /// <param name="sender">element that invokes this handler (e.g an Ellipse)</param>
        /// <param name="e">touch event information such type of device</param>
        private void DataPointTouchEnter(object sender, TouchEventArgs e)
        {
            Ellipse senderEllipse = sender as Ellipse;
            
            haloEllipse.SetValue(Canvas.LeftProperty, Canvas.GetLeft(senderEllipse) - senderEllipse.Width/4);
            haloEllipse.SetValue(Canvas.BottomProperty, Canvas.GetBottom(senderEllipse) - senderEllipse.Height/4);
            haloEllipse.Height = 30;
            haloEllipse.Width = 30;
            haloEllipse.Stroke = SurfaceColors.BulletBrush;
            haloEllipse.StrokeThickness = 2;
            MainCanvas.Children.Add(haloEllipse);

            int submissionIndex = int.Parse(senderEllipse.Name.Split('_')[1]);
            Submission sData = submissionData.Submissions[submissionIndex];
            if (senderEllipse != null)
            {
                LogMsg(sData.UserId);

                //Interactively remove elements that have been touched (could make for a game)
                //MainCanvas.Children.Remove(senderEllipse);
            }

            //This will produce details on demand
            UpdateDescription(RootGrid, e.TouchDevice, sData, true);

            // Capture to the ellipse.  
            //e.TouchDevice.Capture(senderEllipse);

            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Describes the brush's color using RGB values.  
            // Each value has a range of 0-255.
            mySolidColorBrush.Color = SurfaceColors.Accent2Color;

            //Used to change color of touched elements.
            //senderEllipse.Fill = mySolidColorBrush;


            // Mark this event as handled.  
            e.Handled = true;
        }

        /// <summary>
        /// Gets a string that describes the type of touch device on the surface
        /// </summary>
        /// <param name="touchDevice">the touch device to be examined</param>
        /// <returns>a string that describes the type of touch device</returns>
        private static string GetTouchDeviceTypeString(TouchDevice touchDevice)
        {
            if (touchDevice.GetTagData() != TagData.None)
            {
                return "Tag";
            }
            if (touchDevice.GetIsFingerRecognized())
            {
                return "Finger";
            }

            return "Blob";
        }

        /// <summary>
        /// Update the text description with the most recent property values. Position
        /// the textbox so that it does not go offscreen (outside parentGrid). Also
        /// position the connecting line between the touch device and the textbox.
        /// </summary>
        /// <param name="parentGrid">the container for this diagram-
        /// description text will not go outside of this container's bounds</param>
        /// <param name="touchDevice">the touch device to diagram</param>
        /// <param name="showTouchDeviceInfo">Whether or not the touch device info will be visible</param>
        private void UpdateDescription(Grid parentGrid, TouchDevice touchDevice, Submission sData, bool showTouchDeviceInfo)
        {
            // Show or hide the touchDevice info based on showTouchDeviceInfo
            Description.Visibility = showTouchDeviceInfo ? Visibility.Visible : Visibility.Hidden;
            ConnectingLine.Visibility = showTouchDeviceInfo ? Visibility.Visible : Visibility.Hidden;

            if (!showTouchDeviceInfo)
            {
                // Don't need to do the calculations if info isn't going to be shown
                return;
            }

            Point position = touchDevice.GetPosition(parentGrid);
            Rect bounds = new Rect(0, 0, parentGrid.ActualWidth, parentGrid.ActualHeight);
            // Determine where around the touchDevice the description should be displayed.
            // The default position is above and to the left.
            bool isAbove = true;
            bool isLeft = true;

            // Description text for tags is different than non-tags
            double descriptionXDistance;
            bool isTag = touchDevice.GetIsTagRecognized();

            if (isTag)
            {
                descriptionXDistance = tagDescriptionXDistance;
            }
            else
            {
                descriptionXDistance = nonTagDescriptionXDistance;
            }

            // Put description below touchDevice if default location is out of bounds.
            Rect upperLeftBounds = GetDescriptionBounds(position, isAbove, isLeft, descriptionXDistance, descriptionYDistance);
            if (upperLeftBounds.Top < bounds.Top)
            {
                isAbove = false;
            }

            // Put description to the right of the touchDevice if default location is out of bounds.
            if (upperLeftBounds.Left < bounds.Left)
            {
                isLeft = false;
            }

            // Calculate the final bounds that will be used for the textbox position
            // based on the updated isAbove and isLeft values.
            Rect finalBounds = GetDescriptionBounds(position, isAbove, isLeft, descriptionXDistance, descriptionYDistance);
            Canvas.SetLeft(Description, finalBounds.Left);
            Canvas.SetTop(Description, finalBounds.Top);

            // Set the justification of the type in the textbox based
            // on which side of the touchDevice the textbox is on.
            if (isLeft)
            {
                Description.TextAlignment = TextAlignment.Right;
            }
            else
            {
                Description.TextAlignment = TextAlignment.Left;
            }

            // Create the description string.
            StringBuilder descriptionText = new StringBuilder();
            //Show demographic info for submission data
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Respondent id: {0}", sData.UserId));
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Age: {0}", sData.Age.ToString()));
            descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Location: {0}, {1}", sData.Latitude.ToString(), sData.Longitude.ToString()));


            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "RecognizedTypes: {0}", GetTouchDeviceTypeString(touchDevice)));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Id: {0}", touchDevice.Id));

            //// Use the "f1" format specifier to limit the amount of decimal positions shown.
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "X: {0}", position.X.ToString("f1", CultureInfo.InvariantCulture)));
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Y: {0}", position.Y.ToString("f1", CultureInfo.InvariantCulture)));

            // Display "null" for Orientation if the touchDevice does not have an orientation value.
            //string orientationString;
            //double? touchDeviceOrientation = touchDevice.GetOrientation(parentGrid);
            //if (touchDeviceOrientation == null)
            //{
            //    orientationString = "null";
            //}
            //else
            //{
            //    orientationString = ((double)touchDeviceOrientation).ToString("f1", CultureInfo.InvariantCulture);
            //}
            //descriptionText.AppendLine(String.Format(CultureInfo.InvariantCulture, "Orientation: {0}", orientationString));

            //if (touchDevice.GetTagData() != TagData.None)
            //{
            //    //descriptionText.AppendLine("Schema: 0x" + touchDevice.GetTagData().Schema.ToString("x8", CultureInfo.InvariantCulture));
            //    //descriptionText.AppendLine("Series:  0x" + touchDevice.GetTagData().Series.ToString("x16", CultureInfo.InvariantCulture));
            //    //descriptionText.AppendLine("ExtendedValue: 0x" + touchDevice.GetTagData().ExtendedValue.ToString("x16", CultureInfo.InvariantCulture));
            //    descriptionText.AppendLine("Value:  0x" + touchDevice.GetTagData().Value.ToString("x4", CultureInfo.InvariantCulture));
            //}

            // Update the description textbox.
            Description.Text = descriptionText.ToString();

            // Update the line that connects the touchDevice to the description textbox.
            double x2;
            if (isLeft)
            {
                x2 = finalBounds.Right;
            }
            else
            {
                x2 = finalBounds.Left;
            }
            // Position (X1,Y1) is the center of the touchDevice.
            // Position (X2,Y2) is the edge of the description text box.
            ConnectingLine.X1 = position.X;
            ConnectingLine.Y1 = position.Y;
            ConnectingLine.X2 = x2;
            ConnectingLine.Y2 = finalBounds.Top + finalBounds.Height * 0.5;
        }


        /// <summary>
        /// Calculate the bounds of the textbox for the specified location and quadrant.
        /// </summary>
        /// <param name="position">position of the touch device to diagram</param>
        /// <param name="isAbove">specifies if the requested bounds is above or below the touch device position</param>
        /// <param name="isLeft">specifies if the requested bounds is right or left of the touch device position</param>
        /// <param name="xDistance">the x distance from the touch evice position</param>
        /// <param name="yDistance">the y distance from the touch device position</param>
        /// <returns>the bounds at the specified location</returns>
        private Rect GetDescriptionBounds(Point position, bool isAbove, bool isLeft, double xDistance, double yDistance)
        {
            double left;
            if (isLeft)
            {
                left = position.X - xDistance - Description.Width;
            }
            else
            {
                left = position.X + xDistance;
            }
            double top;
            if (isAbove)
            {
                top = position.Y - yDistance - Description.Height;
            }
            else
            {
                top = position.Y + yDistance;
            }
            return new Rect(left, top, Description.Width, Description.Height);
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
