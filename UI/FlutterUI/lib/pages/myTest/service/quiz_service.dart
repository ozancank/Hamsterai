import 'package:dio/dio.dart';
import 'package:mobile/core/base/base_service.dart';
import 'package:mobile/core/enums/service_endpoints.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/myTest/model/quiz_list_model.dart';
import 'package:mobile/pages/myTest/model/quiz_model.dart';

class QuizService extends BaseService {
  QuizService(super.dio);

  Future getQuizzes(Map<String, dynamic> json) async {
    getAuth();
    try {
      final response = await dio.post(
          '${ServiceEndpoints.getQuizzes.getEndpoint}?Page=0&PageSize=0',
          data: json);
      if (response.statusCode == 200) {
        final data = QuizListModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      return false;
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future getQuizQuestions(String id) async {
    getAuth();
    try {
      final response = await dio.get(
        '${ServiceEndpoints.getQuizQuestions.getEndpoint}/$id',
      );
      if (response.statusCode == 200) {
        final data = QuizModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future updateQuiz(Map<String, dynamic> json) async {
    getAuth();
    try {
      final response = await dio.post(
        ServiceEndpoints.updateQuiz.getEndpoint,
        data: json,
      );
      if (response.statusCode == 200) {
        final data = QuizModel.fromJson(response.data);
        return data;
      }
    } on DioException catch (e) {
      final errorJson = e.response?.data;
      return ErrorModel.fromJson(errorJson);
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }

  Future quizStarted(String quizId) async {
    getAuth();
    try {
      final response = await dio.post(
        '${ServiceEndpoints.updateQuizStarted.getEndpoint}?quizId=$quizId',
      );
      if (response.statusCode == 200) {
        final data = response.data;
        return true;
      }
    } on DioException catch (e) {
      return false;
    } catch (e) {
      return ErrorModel(statusCode: 500, message: 'Bilinmeyen bir hata oluştu');
    }
    return ErrorModel(statusCode: 400, message: 'Hata');
  }
}
