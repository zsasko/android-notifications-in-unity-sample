﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NotificationSamples;
using System;
using UnityEngine.UI;
using System.Text;

public class AppNotificationManager : MonoBehaviour
{

    [SerializeField, Tooltip("Reference to the notification manager.")]
    public GameNotificationsManager manager;
    [SerializeField]
    public Text notificationScheduledText;

    private const string NOTIFICATION_CHANNEL_ID = "notification_channel_id";
    private const string GAME_NOTIFICATION_CHANNEL_TITLE = "Get back to the game!";
    private const string GAME_NOTIFICATION_CHANNEL_DESCRIPTION = "Notification from my Game";
    private const int DISPLAY_NOTIFICATION_AFTER_DAYS = 3;

    // notification icons
    private string smallIconName = "icon_0";
    private string largeIconName = "icon_1";

    void Start()
    {
        InitializeGameChannel();
        //
        ScheduleNotificationForUnactivity();
        //
        DisplayPendingNotification();
    }

    private void InitializeGameChannel()
    {
        var channel = new GameNotificationChannel(NOTIFICATION_CHANNEL_ID, GAME_NOTIFICATION_CHANNEL_TITLE, GAME_NOTIFICATION_CHANNEL_DESCRIPTION);
        manager.Initialize(channel);
    }

    private void ScheduleNotificationForUnactivity()
    { 
        // cancelling old notifications
        manager.CancelAllNotifications();
        //
        ScheduleNotificationForUnactivity(DISPLAY_NOTIFICATION_AFTER_DAYS);
    }

    private void ScheduleNotificationForUnactivity(int daysIncrement)
    {
        string title = GAME_NOTIFICATION_CHANNEL_TITLE;
        string description = GAME_NOTIFICATION_CHANNEL_DESCRIPTION;
        DateTime deliveryTime = DateTime.UtcNow.AddDays(daysIncrement);
        string channel = NOTIFICATION_CHANNEL_ID;
        //
        SendNotification(title, description, deliveryTime, channelId: channel, smallIcon: smallIconName, largeIcon: largeIconName);
    }

    public void SendNotification(string title, string body, DateTime deliveryTime, int? badgeNumber = null, bool reschedule = false, string channelId = null, string smallIcon = null, string largeIcon = null)
    {
        IGameNotification notification = manager.CreateNotification();
        if (notification == null)
        {
            return;
        }
        notification.Title = title;
        notification.Body = body;
        notification.Group =
            !string.IsNullOrEmpty(channelId) ? channelId : NOTIFICATION_CHANNEL_ID;
        notification.DeliveryTime = deliveryTime;
        notification.SmallIcon = smallIcon;
        notification.LargeIcon = largeIcon;
        if (badgeNumber != null)
        {
            notification.BadgeNumber = badgeNumber;
        }
        PendingNotification notificationToDisplay = manager.ScheduleNotification(notification);
        notificationToDisplay.Reschedule = reschedule;
        //
        Debug.Log($"Queued notification for unactivity with ID \"{notification.Id}\" at time {deliveryTime:dd.MM.yyyy HH:mm:ss}");
    }

    private void DisplayPendingNotification()
    {
        StringBuilder notificationStringBuilder = new StringBuilder("Pending notifications at:");
        notificationStringBuilder.AppendLine();
        for (int i = manager.PendingNotifications.Count - 1; i >= 0; --i)
        {
            PendingNotification queuedNotification = manager.PendingNotifications[i];
            DateTime? time = queuedNotification.Notification.DeliveryTime;
            if (time != null)
            {
                notificationStringBuilder.Append($"{time:dd.MM.yyyy HH:mm:ss}");
                notificationStringBuilder.AppendLine();
            }
        }
        notificationScheduledText.text = notificationStringBuilder.ToString();
    }
}
