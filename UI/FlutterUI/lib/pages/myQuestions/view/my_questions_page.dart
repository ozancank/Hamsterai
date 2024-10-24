import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/date_extensions.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/dash_divider.dart';
import 'package:mobile/pages/myQuestions/controller/my_questions_controller.dart';
import 'package:mobile/pages/questionDetails/view/question_detail_page.dart';
import 'package:mobile/styles/colors.dart';

class MyQuestionsPage extends StatefulWidget {
  const MyQuestionsPage({super.key});

  @override
  State<MyQuestionsPage> createState() => _MyQuestionsPageState();
}

class _MyQuestionsPageState extends State<MyQuestionsPage> {
  var questionController = Get.put(MyQuestionsController());
  bool isScrolled = false;

  Future<void> refreshData() async {
    questionController.getQuestions();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: const CommonAppbar(title: 'Sorularım'),
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
            padding: const EdgeInsets.symmetric(horizontal: 10),
            decoration: const BoxDecoration(
              color: Colors.white,
            ),
            child: Obx(
              () {
                if (questionController.loading.value) {
                  return const Center(
                    child: CircularProgressIndicator(
                      color: MyColors.primaryColor,
                    ),
                  );
                }
                var questions = questionController.questionList;
                if (questions.isEmpty) {
                  return Center(
                    child: Text(
                      'Gösterilecek soru bulunamadı',
                      style: Theme.of(context).textTheme.bodyLarge,
                    ),
                  );
                }

                return SingleChildScrollView(
                  child: Column(
                    children: [
                      ...List.generate(questionController.questionList.length,
                          (index) {
                        var questions = questionController.questionList;
                        return GestureDetector(
                          onTap: () {
                            HapticFeedback.mediumImpact();
                            questionController
                                .readQuestion(questions[index].id);
                            Get.to(() => QuestionDetailPage(
                                  question: questions[index],
                                ));
                          },
                          child: Container(
                            padding: const EdgeInsets.only(bottom: 7, top: 5),
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
                                        color: questions[index].status == 3
                                            ? Colors.red
                                            : questions[index].status == 1
                                                ? const Color(0xFFD9D9D9)
                                                : MyColors.primaryColor,
                                      ),
                                      child: Icon(
                                        questions[index].status == 3
                                            ? Icons.close
                                            : questions[index].status == 1
                                                ? Icons.access_alarms_sharp
                                                : Icons.done,
                                        color: Colors.white,
                                      ),
                                    ),
                                    const SizedBox(width: 15),
                                    Column(
                                      crossAxisAlignment:
                                          CrossAxisAlignment.start,
                                      children: [
                                        Text(
                                          questionController
                                              .questionList[index].lessonName,
                                          style: Theme.of(context)
                                              .textTheme
                                              .titleMedium!
                                              .copyWith(
                                                color: MyColors.darkBlue,
                                              ),
                                        ),
                                        Text(
                                          questionController.questionList[index]
                                                      .status ==
                                                  2
                                              ? 'Tamamlandı - ${questionController.questionList[index].createDate.toString().toFormattedDateTime()}'
                                              : questionController
                                                          .questionList[index]
                                                          .status ==
                                                      3
                                                  ? 'Servis tarafından hata verildi.'
                                                  : 'Çözüm devam ediyor..',
                                          overflow: TextOverflow.ellipsis,
                                          style: Theme.of(context)
                                              .textTheme
                                              .bodySmall,
                                        ),
                                      ],
                                    ),
                                  ],
                                ),
                                const SizedBox(height: 7),
                                // Row(
                                //   children: [
                                //     Expanded(
                                //       child: SvgPicture.asset(
                                //         AssetsConstant.threeDots,
                                //         height: 30,
                                //       ),
                                //     ),
                                //     Expanded(flex: 6, child: Container()),
                                //   ],
                                // ),
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
                      }),
                    ],
                  ),
                );
              },
            ),
          ),
        ),
      ),
    );
  }
}
