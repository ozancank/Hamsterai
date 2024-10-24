enum ServiceEndpoints {
  login,
  logout,
  addQuestion,
  getQuestions,
  getUserById,
  passwordChangeByUser,
  getLessons,
  forgotPassword,
  addDeviceToken,
  addSimilarQuestion,
  getSimilarQuestion,
  updateUser,
  updatePassword,
  getStudentGains,
  getQuizzes,
  getQuizQuestions,
  updateQuiz,
  updateQuizStarted,
  isReadQuestion,
  getHomeworks,
  getStudentById,
}

extension ServiceEndpointsExtension on ServiceEndpoints {
  String get _rawValue {
    switch (this) {
      case ServiceEndpoints.login:
        return '/auth/login';
      case ServiceEndpoints.logout:
        return '/auth/logout';
      case ServiceEndpoints.addQuestion:
        return '/Question/AddQuestion';
      case ServiceEndpoints.getQuestions:
        return '/Question/GetQuestions';
      case ServiceEndpoints.getUserById:
        return '/User/GetUserById';
      case ServiceEndpoints.passwordChangeByUser:
        return '/User/PasswordChangeByUser';
      case ServiceEndpoints.getLessons:
        return '/Lesson/GetLessons';
      case ServiceEndpoints.forgotPassword:
        return '/User/UserSendForgetPasswordEmail';
      case ServiceEndpoints.addDeviceToken:
        return '/Notification/AddDeviceToken';
      case ServiceEndpoints.addSimilarQuestion:
        return '/Question/AddSimilarQuestion';
      case ServiceEndpoints.getSimilarQuestion:
        return '/Question/GetSimilarQuestions';
      case ServiceEndpoints.updateUser:
        return '/User/UpdateUser';
      case ServiceEndpoints.updatePassword:
        return '/User/PasswordChangeByUser';
      case ServiceEndpoints.getStudentGains:
        return '/Student/GetStudentGainsForSelf';
      case ServiceEndpoints.getQuizzes:
        return '/Quiz/GetQuizzes';
      case ServiceEndpoints.getQuizQuestions:
        return '/Quiz/GetQuizById';
      case ServiceEndpoints.updateQuiz:
        return '/Quiz/UpdateQuiz';
      case ServiceEndpoints.updateQuizStarted:
        return '/Quiz/UpdateQuizStarted';
      case ServiceEndpoints.isReadQuestion:
        return '/Question/IsReadQuestion';
      case ServiceEndpoints.getHomeworks:
        return '/Homework/GetHomeworksForStudent';
      case ServiceEndpoints.getStudentById:
        return '/Student/GetStudentById';
    }
  }

  String get getEndpoint {
    return '/v1$_rawValue';
  }
}
