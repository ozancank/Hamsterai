import 'dart:async';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/core/init/network/network_manager.dart';
import 'package:mobile/pages/auth/model/error_model.dart';
import 'package:mobile/pages/myTest/model/quiz_list_model.dart';
import 'package:mobile/pages/myTest/model/quiz_model.dart';
import 'package:mobile/pages/myTest/model/update_quiz_post_model.dart';
import 'package:mobile/pages/myTest/service/quiz_service.dart';
import 'package:shared_preferences/shared_preferences.dart';

class QuizController extends GetxController {
  late QuizService _quizService;
  RxList<dynamic> quizList = [].obs;
  RxList<dynamic> quizQuestionList = [].obs;
  Rx<QuizModel?> quizModel = Rx<QuizModel?>(null);
  var loading = false.obs;
  late List<dynamic> selectedAnswers;
  var lessonId = 0.obs;
  var savedAnswer;
  Timer? testTimer;
  RxInt elapsedTime = 0.obs;
  var isTimerRunning = false.obs;
  RxBool isEraserMode = false.obs;

  @override
  void onInit() {
    _quizService = QuizService(NetworkManager.instance.dio);
    selectedAnswers = [];
    getQuizList();
    super.onInit();
  }

  void toggleEraser() {
    isEraserMode.value = !isEraserMode.value;
  }

  Future<void> saveAnswer(
      String quizId, int questionNumber, String selectedOption) async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    prefs.setString('quiz_$quizId-$questionNumber', selectedOption);
    selectedAnswers[questionNumber] = selectedOption;
  }

  Future<void> loadAnswers(String quizId) async {
    SharedPreferences prefs = await SharedPreferences.getInstance();
    if (quizQuestionList.isNotEmpty) {
      selectedAnswers = List<String?>.filled(quizQuestionList.length, null);
      for (int i = 0; i < quizQuestionList.length; i++) {
        String key = 'quiz_$quizId-$i';
        savedAnswer = prefs.getString(key);
        if (savedAnswer != null) {
          selectedAnswers[i] = savedAnswer;
        }
      }
    } else {
      selectedAnswers = List<String?>.filled(quizQuestionList.length, null);
    }
  }

  String? getSelectedAnswer(int questionNumber) {
    if (questionNumber <= selectedAnswers.length + 1) {
      return selectedAnswers[questionNumber];
    }
    return null;
  }

  Future<void> getQuizList() async {
    loading.value = true;
    var today = DateTime.now();
    var startDate = today.subtract(const Duration(days: 2));
    var getQuiz = await _quizService.getQuizzes({
      "lessonId": lessonId.value,
      "startDate": null,
      "endDate": null,
    });
    if (getQuiz is QuizListModel) {
      loading.value = false;
      quizList.value = getQuiz.items;
    } else if (getQuiz is ErrorModel) {
      loading.value = false;
    }
  }

  Future<void> getQuizQuestions(String id) async {
    loading.value = true;
    var getQuizQuestions = await _quizService.getQuizQuestions(id);
    if (getQuizQuestions is QuizModel) {
      loading.value = false;
      quizModel.value = getQuizQuestions;
      selectedAnswers =
          List<String?>.filled(getQuizQuestions.questions.length, null);
      quizQuestionList.value = getQuizQuestions.questions;
      await loadAnswers(id);
    } else if (getQuizQuestions is ErrorModel) {
      loading.value = false;
    }
  }

  void startTimer() {
    if (!isTimerRunning.value) {
      elapsedTime.value = 0;
      testTimer = Timer.periodic(const Duration(seconds: 1), (timer) {
        elapsedTime.value++;
      });
      isTimerRunning.value = true;
    }
  }

  void stopTimer() {
    if (isTimerRunning.value) {
      testTimer?.cancel();
      isTimerRunning.value = false;
    }
  }

  String get formattedTime {
    final int minutes = elapsedTime.value ~/ 60;
    final int seconds = elapsedTime.value % 60;
    return "${minutes.toString().padLeft(2, '0')}:${seconds.toString().padLeft(2, '0')}";
  }

  void resetTimer() {
    stopTimer();
    elapsedTime.value = 0;
  }

  Future<void> updateQuiz(String quizId, {int? questionIndex}) async {
    loading.value = true;

    List<Answer> answers = [];
    if (questionIndex == 1 && selectedAnswers[0] == null) {
      return;
    }
    if (questionIndex != null) {
      for (int i = 0; i < questionIndex; i++) {
        if (selectedAnswers[i] != null) {
          answers.add(Answer(
            questionId: quizQuestionList[i].id,
            answerOption: selectedAnswers[i]!,
          ));
        }
      }
    } else {
      for (int i = 0; i < quizQuestionList.length; i++) {
        if (selectedAnswers[i] != null) {
          answers.add(Answer(
            questionId: quizQuestionList[i].id,
            answerOption: selectedAnswers[i]!,
          ));
        }
      }
    }

    var tempJson = UpdateQuizPostModel(
      quizId: quizId,
      timeSecond: elapsedTime.value,
      status: questionIndex != null ? 3 : 4,
      answers: answers,
    ).toJson();

    var response = await _quizService.updateQuiz(tempJson);
    stopTimer();

    if (response is QuizModel) {
      loading.value = false;
      quizModel.value = response;
    } else if (response is ErrorModel) {
      loading.value = false;
    }
  }

  Future<void> quizStarted(String quizId) async {
    EasyLoading.show(
        status: 'Teste giriş Yapılıyor', maskType: EasyLoadingMaskType.black);
    loading.value = true;
    var getQuizQuestions = await _quizService.quizStarted(quizId);
    if (getQuizQuestions) {
      EasyLoading.dismiss();
      startTimer();
      loading.value = false;
    } else if (getQuizQuestions is ErrorModel) {
      EasyLoading.dismiss();
      loading.value = false;
    }
  }
}
