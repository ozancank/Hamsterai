import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/dash_divider.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';
import 'package:mobile/pages/myTest/view/test_login_view.dart';
import 'package:mobile/styles/colors.dart';

class MyTestView extends StatefulWidget {
  const MyTestView({super.key});

  @override
  State<MyTestView> createState() => _MyTestViewState();
}

class _MyTestViewState extends State<MyTestView> {
  var quizListController = Get.put(QuizController());

  Future<void> refreshData() async {
    quizListController.getQuizList();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar(title: 'Testlerim'),
      backgroundColor: Colors.white,
      body: Container(
        color: Colors.white,
        padding: const EdgeInsets.only(top: 10),
        margin: EdgeInsets.symmetric(
          horizontal: context.dynamicWidth * 0.06,
        ),
        child: RefreshIndicator(
          color: MyColors.primaryColor,
          onRefresh: refreshData,
          child: Container(
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              color: Colors.white,
              borderRadius: BorderRadius.circular(20),
              // boxShadow: [
              //   BoxShadow(
              //     offset: const Offset(0, 24),
              //     blurRadius: 48,
              //     color: const Color(0xFFB3B8BF).withOpacity(0.3),
              //     spreadRadius: -16,
              //   ),
              // ],
            ),
            child: Obx(() {
              if (quizListController.loading.value) {
                return const Center(
                  child: CircularProgressIndicator(
                    color: MyColors.primaryColor,
                  ),
                );
              }
              var questions = quizListController.quizList;
              if (questions.isEmpty) {
                return Center(
                  child: Text(
                    'Gösterilecek test bulunamadı',
                    style: Theme.of(context).textTheme.bodyLarge,
                  ),
                );
              }

              return ListView.builder(
                itemCount: quizListController.quizList.length,
                itemBuilder: (context, index) {
                  print(questions[index].status);
                  return GestureDetector(
                    onTap: () {
                      HapticFeedback.mediumImpact();
                      quizListController.getQuizQuestions(
                          quizListController.quizList[index].id);
                      Get.to(() => TestLoginView(
                            quizItem: quizListController.quizList[index],
                          ));
                    },
                    child: Container(
                      padding: const EdgeInsets.only(bottom: 7),
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Row(
                            children: [
                              Container(
                                alignment: Alignment.center,
                                padding: const EdgeInsets.all(10),
                                decoration: BoxDecoration(
                                  shape: BoxShape.circle,
                                  color: questions[index].status == 2 ||
                                          questions[index].status == 3 ||
                                          questions[index].status == 1
                                      ? const Color(0xFFD9D9D9)
                                      : questions[index].status == 4
                                          ? MyColors.greenColor2
                                          : MyColors.primaryColor,
                                ),
                                child: Icon(
                                  questions[index].status == 2 ||
                                          questions[index].status == 3
                                      ? Icons.hourglass_bottom_rounded
                                      : questions[index].status == 4
                                          ? Icons.done_all_rounded
                                          : Icons.access_time_rounded,
                                  color: Colors.white,
                                ),
                              ),
                              const SizedBox(width: 15),
                              Column(
                                crossAxisAlignment: CrossAxisAlignment.start,
                                children: [
                                  Text(
                                    quizListController
                                            .quizList[index].lessonName +
                                        ' Testi',
                                    style: Theme.of(context)
                                        .textTheme
                                        .titleMedium!
                                        .copyWith(
                                          color: MyColors.darkBlue,
                                        ),
                                  ),
                                  Text(
                                    questions[index].status == 4
                                        ? 'Tamamlandı'
                                        : questions[index].status == 3 ||
                                                questions[index].status == 2
                                            ? 'Teste ara verildi'
                                            : 'Çözülmesi bekleniyor',
                                    style: Theme.of(context)
                                        .textTheme
                                        .titleSmall!
                                        .copyWith(
                                          color: MyColors.darkBlue,
                                        ),
                                  )
                                ],
                              ),
                            ],
                          ),
                          Padding(
                            padding: const EdgeInsets.all(8.0),
                            child: DashedDivider(
                              dashWidth: 5,
                              dashSpace: 3,
                              color: Colors.grey.withOpacity(0.3),
                            ),
                          ),
                        ],
                      ),
                    ),
                  );
                },
              );
            }),
          ),
        ),
      ),
    );
  }
}
