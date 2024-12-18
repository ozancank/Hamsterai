import 'package:get/get.dart';
import 'package:mobile/pages/auth/view/login_view.dart';
import 'package:mobile/pages/auth/controller/bindings.dart';
import 'package:mobile/pages/auth/view/update_password_force_view.dart';
import 'package:mobile/pages/home/controller/home_page_binding.dart';
import 'package:mobile/pages/home/view/home_page.dart';
import 'package:mobile/pages/homeworks/controller/homework_binding.dart';
import 'package:mobile/pages/homeworks/view/homework_view.dart';
import 'package:mobile/pages/myQuestions/controller/my_questions_binding.dart';
import 'package:mobile/pages/myQuestions/view/my_questions_page.dart';
import 'package:mobile/pages/myTest/controller/quiz_binding.dart';
import 'package:mobile/pages/myTest/view/my_test_view.dart';
import 'package:mobile/pages/profileDetail/controller/profile_detail_binding.dart';
import 'package:mobile/pages/profileDetail/view/profile_detail_page.dart';
import 'package:mobile/pages/questionDetails/controller/question_detail_binding.dart';
import 'package:mobile/pages/questionDetails/view/question_detail_page.dart';
import 'package:mobile/pages/scan/controller/scan_binding.dart';
import 'package:mobile/pages/scan/model/get_question_model.dart';
import 'package:mobile/pages/scan/view/scan_page.dart';
import 'package:mobile/pages/settings/controller/settings_binding.dart';
import 'package:mobile/pages/settings/view/settings_page.dart';
import 'package:mobile/pages/similarQuestions/controller/similar_question_binding.dart';
import 'package:mobile/pages/similarQuestions/view/similar_question_page.dart';
import 'package:mobile/pages/splash/splash.dart';
import 'package:mobile/pages/splash/splash_binding.dart';
import 'package:mobile/pages/updatePassword/controller/update_password_binding.dart';
import 'package:mobile/pages/updatePassword/view/update_password_page.dart';
part './routes.dart';

abstract class AppPages {
  static final pages = [
    GetPage(
      name: Routes.LOGIN,
      page: () => const LoginView(),
      binding: LoginBinding(),
    ),
    GetPage(
      name: Routes.SPLASH,
      page: () => const SplashPage(),
      binding: SplashBinding(),
    ),
    GetPage(
      name: Routes.HOME,
      page: () => HomePage(),
      binding: HomePageBinding(),
    ),
    GetPage(
      name: Routes.SCAN,
      page: () => const ScanPage(),
      binding: ScanBinding(),
    ),
    GetPage(
      name: Routes.QUESTION_DETAIL,
      page: () => QuestionDetailPage(
        baseUrl: '',
        question: Question.fromJson({}),
      ),
      binding: QuestionDetailBinding(),
    ),
    GetPage(
      name: Routes.SETTINGS,
      page: () => const SettingsPage(),
      binding: SettingsBinding(),
    ),
    GetPage(
      name: Routes.PROFILE_DETAIL,
      page: () => const ProfileDetailPage(),
      binding: ProfileDetailBinding(),
    ),
    GetPage(
      name: Routes.UPDATE_PASSWORD,
      page: () => const UpdatePasswordPage(),
      binding: UpdatePasswordBinding(),
    ),
    GetPage(
      name: Routes.SIMILAR_QUESTIONS,
      page: () => const SimilarQuestionPage(),
      binding: SimilarQuestionBinding(),
    ),
    GetPage(
      name: Routes.MY_TEST,
      page: () => const MyTestView(),
      binding: QuizBinding(),
    ),
    GetPage(
      name: Routes.HOMEWORK,
      page: () => const HomeworkView(
        appBarText: '',
      ),
      binding: HomeworkBinding(),
    ),
    GetPage(
      name: Routes.FORCEUPDATEPASSWORD,
      page: () => const UpdatePasswordForceView(),
      binding: LoginBinding(),
    ),
    GetPage(
      name: Routes.MYQUESTIONPAGE,
      page: () => const MyQuestionsPage(),
      binding: MyQuestionsBinding(),
    ),
  ];
}
