import axios from 'axios';

export default {
  async getNotifications(userId) {
    return await axios.get('Notification/user/' + userId);
  },
  async getTotalNotifications(userId) {
    return await axios.get('Notification/user-total/' + userId);
  },
  async markNotificationAsSeen(notificationIds, userId) {
    return await axios.post('Notification/seen', {
      notificationIds, userId
    });
  },
  async createNotification(payload) {
    return await axios.post('Notification', payload);
  },
  async deleteNotification(payload) {
    return await axios.delete('Notification', payload);
  }
}