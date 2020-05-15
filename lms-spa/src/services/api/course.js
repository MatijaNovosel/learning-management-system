import axios from 'axios';

export default {
  /**
   * Retrieves a list of courses, filtered by function parameters.
   * @param {number} userId - User id, sent in order to check for subscribed courses.
   * @param {number} specializationId - Specialization id.
   * @param {string} name - Course name.
   * @param {boolean} subscribed - Filter by subscribed courses.
   * @param {boolean} nonSubscribed - Filter by non subscribed courses.
   * @return {AxiosPromise<any>} Axios promise to be resolved in the view.
   */
  async get(userId, specializationId, name, subscribed, nonSubscribed) {
    return await axios.get('Course', {
      params: {
        userId,
        specializationId,
        name,
        subscribed,
        nonSubscribed
      }
    });
  },
  /**
   * Retrieves the details of a specific course.
   * @param {number} id - Id of the requested course.
   * @return {AxiosPromise<any>} Axios promise to be resolved in the view.
   */
  async getCourse(id) {
    return await axios.get(`Course/${id}`);
  },
  /**
   * Retrieves a list of sidebars of a specific course.
   * @param {number} id - Id of the requested course.
   * @return {AxiosPromise<any>} Axios promise to be resolved in the view.
   */
  async getCourseSidebar(id) {
    return await axios.get(`Course/sidebar/${id}`);
  },
  /**
   * Retrieves a list of notifications for a specific course.
   * @param {number} courseId - Id of the requested course.
   * @return {AxiosPromise<any>} Axios promise to be resolved in the view.
   */
  async getNotifications(courseId, showArchived, showNonArchived) {
    return await axios.get(`Course/notifications/${courseId}`, {
      params: {
        showArchived,
        showNonArchived
      }
    });
  },
  async getCourseUsers(courseId, name, surname, username) {
    return await axios.get(`Course/users/${courseId}`, { params: { name, surname, username } });
  }
}