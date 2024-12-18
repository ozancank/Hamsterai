import 'package:flutter/material.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/common/result_container.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';
import 'package:mobile/pages/myTest/view/test_result_detail_view.dart';
import 'package:mobile/styles/colors.dart';

class TestResultView extends StatefulWidget {
  const TestResultView({super.key});

  @override
  State<TestResultView> createState() => _TestResultViewState();
}

class _TestResultViewState extends State<TestResultView> {
  final quizController = Get.put(QuizController());
  int correctCount = 0;
  int incorrectCount = 0;
  Map<String, Map<String, int>> gainResults = {};
  int emptyCount = 0;
  Map<String, Map<String, int>> emptyGainResults = {};
  @override
  void initState() {
    super.initState();
    calculateGainResults();
  }

  void calculateGainResults() {
    gainResults.clear();
    for (int i = 0; i < quizController.quizModel.value!.questions.length; i++) {
      String gainName =
          quizController.quizModel.value!.questions[i].gainName != null
              ? quizController.quizModel.value!.questions[i].gainName!
              : '';
      bool rightAnswer =
          quizController.quizModel.value!.answers.values.elementAt(i) ==
              quizController.quizModel.value!.rightOptions.values.elementAt(i);

      if (!gainResults.containsKey(gainName)) {
        gainResults[gainName] = {'correct': 0, 'incorrect': 0};
      }

      if (rightAnswer) {
        gainResults[gainName]!['correct'] =
            gainResults[gainName]!['correct']! + 1;
      } else {
        gainResults[gainName]!['incorrect'] =
            gainResults[gainName]!['incorrect']! + 1;
      }
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      backgroundColor: Colors.white,
      body: Padding(
        padding: EdgeInsets.symmetric(horizontal: context.dynamicWidth * 0.05),
        child: SingleChildScrollView(
          child: Column(
            children: [
              SizedBox(height: context.height * 0.09),
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(
                    color: MyColors.primaryColor.withOpacity(0.3),
                  ),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    SvgPicture.asset(
                      AssetsConstant.list,
                      color: MyColors.primaryColor,
                    ),
                    const SizedBox(width: 10),
                    Text(
                      // '${quizController.quizModel.value?.lessonName} Testi Sonuçları',
                      'Test Sonuçları',
                      style: Theme.of(context).textTheme.titleLarge!.copyWith(
                            color: MyColors.primaryColor,
                            fontWeight: FontWeight.w500,
                          ),
                    ),
                  ],
                ),
              ),
              SizedBox(height: context.height * 0.03),
              // Container(
              //   padding: const EdgeInsets.all(5),
              //   decoration: BoxDecoration(
              //     borderRadius: BorderRadius.circular(30),
              //   ),
              //   child: Padding(
              //     padding:
              //         EdgeInsets.symmetric(horizontal: context.width * 0.03),
              //     child: Row(
              //       mainAxisAlignment: MainAxisAlignment.end,
              //       children: [
              //         //  buildContainer(context, 'Sıra'),
              //         //    buildContainer(context, 'Cevap'),
              //         buildContainer(context, 'Cevabınız'),
              //         SizedBox(width: 100),
              //         // buildContainer(context, 'Sonuç'),
              //       ],
              //     ),
              //   ),
              // ),
              Obx(() {
                int correctCount = 0;
                int incorrectCount = 0;

                for (int i = 0;
                    i < quizController.selectedAnswers.length;
                    i++) {
                  bool rightAnswer = quizController
                          .quizModel.value!.answers.values
                          .elementAt(i) ==
                      quizController.quizModel.value!.rightOptions.values
                          .elementAt(i);
                  emptyCount = quizController.quizModel.value!.emptyCount;
                  incorrectCount = quizController.quizModel.value!.wrongCount;
                  correctCount = quizController.quizModel.value!.correctCount;
                  if (quizController.quizModel.value!.answers.values
                          .elementAt(i) ==
                      null) {
                    String gainName =
                        quizController.quizQuestionList[i].gainName ?? '';

                    if (!emptyGainResults.containsKey(gainName)) {
                      emptyGainResults[gainName] = {};
                    }

                    emptyGainResults[gainName]!['count'] =
                        (emptyGainResults[gainName]!['count'] ?? 0) + 1;
                  }
                }

                return Column(
                  children: [
                    SingleChildScrollView(
                      child: Column(
                        children: List.generate(
                          quizController
                              .quizModel.value!.rightOptions.values.length,
                          (index) {
                            bool emptyAnswer = quizController
                                    .quizModel.value!.answers.values
                                    .elementAt(index) ==
                                null;
                            bool rightAnswer = quizController
                                    .quizModel.value!.answers.values
                                    .elementAt(index) ==
                                quizController
                                    .quizModel.value!.rightOptions.values
                                    .elementAt(index);
                            print(quizController
                                .quizQuestionList[index].answerOption);
                            print(emptyAnswer);
                            return Column(
                              children: [
                                GestureDetector(
                                  onTap: () {
                                    Get.to(() =>
                                        TestResultDetailView(index: index));
                                  },
                                  child: Container(
                                    margin: const EdgeInsets.symmetric(
                                      vertical: 5,
                                      horizontal: 5,
                                    ),
                                    decoration: BoxDecoration(
                                      color: rightAnswer
                                          ? const Color(0xFFA0D94A)
                                              .withOpacity(0.6)
                                          : emptyAnswer
                                              ? const Color(0xFFD9D9D9)
                                              : const Color(0xFFF76A6A),
                                      borderRadius: BorderRadius.circular(30),
                                    ),
                                    child: Padding(
                                      padding: EdgeInsets.symmetric(
                                          horizontal: context.width * 0.05,
                                          vertical: 5),
                                      child: Row(
                                        mainAxisAlignment:
                                            MainAxisAlignment.spaceBetween,
                                        children: [
                                          questionRows(
                                              context, (index + 1).toString()),
                                          Obx(() {
                                            return Row(
                                              mainAxisAlignment:
                                                  MainAxisAlignment.spaceEvenly,
                                              children: List.generate(
                                                quizController
                                                    .quizQuestionList[index]
                                                    .optionCount,
                                                (optionIndex) {
                                                  String label =
                                                      String.fromCharCode(
                                                          65 + optionIndex);
                                                  return _buildSelectableContainer(
                                                    label,
                                                    index,
                                                    quizController
                                                        .quizModel.value!.id,
                                                    rightAnswer,
                                                    emptyAnswer,
                                                  );
                                                },
                                              ),
                                            );
                                          }),
                                          SvgPicture.asset(
                                            AssetsConstant.rightBack,
                                            color: Colors.white,
                                          ),
                                        ],
                                      ),
                                    ),
                                  ),
                                ),
                                SingleChildScrollView(
                                  scrollDirection: Axis.horizontal,
                                  child: Row(
                                    mainAxisAlignment: MainAxisAlignment.start,
                                    children: [
                                      SizedBox(
                                        width: context.width * 0.1,
                                      ),
                                      SvgPicture.asset(
                                        AssetsConstant.gainOk,
                                        color: rightAnswer
                                            ? const Color(0xFF94D073)
                                            : quizController
                                                        .quizQuestionList[index]
                                                        .answerOption ==
                                                    null
                                                ? const Color(0xFFD9D9D9)
                                                : const Color(0xFFDD5050),
                                      ),
                                      const SizedBox(width: 10),
                                      Container(
                                        width: context.width * 0.65,
                                        margin: const EdgeInsets.only(top: 10),
                                        padding: EdgeInsets.only(
                                          left: context.width * 0.03,
                                          right: context.width * 0.03,
                                          top: 2,
                                          bottom: 2,
                                        ),
                                        decoration: BoxDecoration(
                                          borderRadius:
                                              BorderRadius.circular(30),
                                          color: rightAnswer
                                              ? const Color(0xFFA0D94A)
                                                  .withOpacity(0.6)
                                              : emptyAnswer
                                                  ? const Color(0xFFD9D9D9)
                                                  : const Color(0xFFF76A6A),
                                        ),
                                        child: SingleChildScrollView(
                                          scrollDirection: Axis.horizontal,
                                          child: Row(
                                            mainAxisAlignment:
                                                MainAxisAlignment.spaceEvenly,
                                            children: [
                                              Text(
                                                quizController
                                                        .quizQuestionList[index]
                                                        .gainName ??
                                                    '',
                                                style: Theme.of(context)
                                                    .textTheme
                                                    .bodySmall!
                                                    .copyWith(
                                                      color: Colors.white,
                                                      fontWeight:
                                                          FontWeight.w700,
                                                      fontSize: 13,
                                                    ),
                                              ),
                                              const SizedBox(
                                                width: 10,
                                              ),
                                              Container(
                                                decoration: const BoxDecoration(
                                                  color: Colors.white,
                                                  shape: BoxShape.circle,
                                                ),
                                                alignment: Alignment.center,
                                                child: Icon(
                                                  rightAnswer
                                                      ? Icons.done_rounded
                                                      : Icons.close,
                                                  color: rightAnswer
                                                      ? const Color(0xFF79A637)
                                                      : emptyAnswer
                                                          ? const Color(
                                                              0xFF808080)
                                                          : const Color(
                                                              0xFFF76A6A),
                                                  size: 20,
                                                ),
                                              ),
                                            ],
                                          ),
                                        ),
                                      ),
                                    ],
                                  ),
                                ),
                                const SizedBox(
                                  height: 5,
                                )
                              ],
                            );
                          },
                        ),
                      ),
                    ),

                    SizedBox(height: context.height * 0.1),
                    Align(
                      alignment: Alignment.centerRight,
                      child: Column(
                        crossAxisAlignment: CrossAxisAlignment.end,
                        children: [
                          ResultContainer(
                            text:
                                'Toplam Soru: ${quizController.quizQuestionList.length} adet',
                            iconData: Icons.list_rounded,
                            onTap: () {},
                            gainNames: gainResults,
                            containerName: 'toplam',
                          ),
                          ResultContainer(
                            text: 'Doğru: $correctCount adet',
                            iconData: Icons.library_add_check_rounded,
                            onTap: () {},
                            gainNames: gainResults,
                            containerName: 'dogru',
                          ),
                          ResultContainer(
                            text: 'Yanlış: $incorrectCount adet',
                            iconData:
                                Icons.do_not_disturb_on_total_silence_outlined,
                            onTap: () {},
                            gainNames: gainResults,
                            containerName: 'yanlis',
                          ),
                          ResultContainer(
                            text: 'Boş: $emptyCount adet',
                            iconData: Icons.list_rounded,
                            onTap: () {},
                            gainNames: emptyGainResults,
                            containerName: 'bos',
                          ),
                        ],
                      ),
                    ),
                    // // Kazanımları listeleme
                    // ListView.builder(
                    //   shrinkWrap: true,
                    //   physics: const NeverScrollableScrollPhysics(),
                    //   itemCount: gainResults.length,
                    //   itemBuilder: (context, index) {
                    //     String gainName = gainResults.keys.elementAt(index);
                    //     int correct = gainResults[gainName]!['correct']!;
                    //     int incorrect = gainResults[gainName]!['incorrect']!;
                    //     return ListTile(
                    //       title: Text(
                    //         gainName,
                    //         style: Theme.of(context)
                    //             .textTheme
                    //             .bodyMedium!
                    //             .copyWith(fontWeight: FontWeight.w600),
                    //       ),
                    //       subtitle: Text(
                    //         'Doğru: $correct | Yanlış: $incorrect',
                    //         style: Theme.of(context)
                    //             .textTheme
                    //             .bodyMedium!
                    //             .copyWith(fontWeight: FontWeight.w500),
                    //       ),
                    //     );
                    //   },
                    // ),
                  ],
                );
              }),
              SizedBox(height: context.dynamicHeight * 0.01),
              Align(
                alignment: Alignment.centerRight,
                child: GestureDetector(
                  onTap: () {
                    quizController.lessonId.value =
                        quizController.quizModel.value!.lessonId;
                    Get.offNamedUntil('/home', ModalRoute.withName('/home'));
                  },
                  child: Container(
                    padding: EdgeInsets.only(
                        right: context.width * 0.08,
                        top: 5,
                        bottom: 5,
                        left: 5),
                    margin: EdgeInsets.symmetric(
                        vertical: context.height * 0.04,
                        horizontal: context.width * 0.07),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(40),
                      color: MyColors.primaryColor,
                    ),
                    child: Row(
                      mainAxisSize: MainAxisSize.min,
                      children: [
                        Container(
                          height: context.height * 0.06,
                          alignment: Alignment.center,
                          padding: const EdgeInsets.all(5),
                          decoration: const BoxDecoration(
                            color: Colors.white,
                            shape: BoxShape.circle,
                          ),
                          child: SvgPicture.asset(
                            AssetsConstant.testeBasla,
                            color: MyColors.primaryColor,
                          ),
                        ),
                        const SizedBox(width: 10),
                        Text(
                          'ANASAYFA',
                          style:
                              Theme.of(context).textTheme.bodyLarge!.copyWith(
                                    color: Colors.white,
                                    fontWeight: FontWeight.w900,
                                  ),
                        ),
                      ],
                    ),
                  ),
                ),
              ),
              // CommonButton(
              //   textColor: Colors.white,
              //   bgcolor: MyColors.primaryColor,
              //   onTap: () {
              //     quizController.lessonId.value =
              //         quizController.quizModel.value!.lessonId;
              //     Get.offNamedUntil('/home', ModalRoute.withName('/home'));
              //   },
              //   text: 'Anasayfa',
              // ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildSelectableContainer(String label, int questionNumber,
      String quizId, bool rightAnswer, bool wrongAnswer) {
    String rightOption =
        quizController.quizQuestionList[questionNumber].rightOption;
    String selectedAnswer = quizController.quizModel.value!.answers.values
            .elementAt(questionNumber) ??
        '';

    Color getBackgroundColor() {
      if (label == rightOption) {
        return const Color(0xFF79A637);
      } else if (selectedAnswer == null && wrongAnswer) {
        return const Color(0xFF808080);
      } else if (wrongAnswer != false) {
        return Colors.transparent;
      } else if (label == selectedAnswer && selectedAnswer != rightOption) {
        return const Color(0xFF808080);
      } else if (rightAnswer == true && label == rightOption) {
        return const Color(0xFF808080);
      } else {
        return Colors.transparent;
      }
    }

    Color getTextColor() {
      if (label == rightOption || label == selectedAnswer) {
        return Colors.white;
      } else {
        return Colors.white;
      }
    }

    return Padding(
      padding: EdgeInsets.symmetric(horizontal: context.width * 0.01),
      child: Container(
        padding: const EdgeInsets.all(10),
        decoration: BoxDecoration(
          shape: BoxShape.circle,
          color: getBackgroundColor(),
          border: Border.all(color: const Color(0xFFFFFFFF)),
        ),
        child: Center(
          child: Text(
            label,
            style: Theme.of(context).textTheme.bodyLarge!.copyWith(
                  color: getTextColor(),
                  fontWeight: FontWeight.w700,
                  fontSize: 14,
                ),
          ),
        ),
      ),
    );
  }

  Container questionRows(BuildContext context, dynamic text) {
    return Container(
      padding: const EdgeInsets.all(10),
      decoration: const BoxDecoration(
        color: Colors.white,
        shape: BoxShape.circle,
      ),
      child: Center(
        child: Text(
          text ?? '',
          style: Theme.of(context).textTheme.bodyLarge!.copyWith(
              fontWeight: FontWeight.w900,
              color: Colors.black.withOpacity(0.7)),
        ),
      ),
    );
  }

  Container questionWrongRows(BuildContext context, dynamic text) {
    return Container(
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: Colors.transparent,
        shape: BoxShape.circle,
        border: Border.all(color: Colors.white, width: 2),
      ),
      child: Center(
        child: Text(
          text ?? '',
          style: Theme.of(context)
              .textTheme
              .bodySmall!
              .copyWith(fontWeight: FontWeight.w900, color: Colors.white),
        ),
      ),
    );
  }

  Container buildContainer(BuildContext context, String text) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 15, vertical: 5),
      decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(30),
          border: Border.all(color: Colors.grey.withOpacity(0.3))),
      child: Center(
        child: Text(
          text,
          style: Theme.of(context)
              .textTheme
              .bodySmall!
              .copyWith(fontWeight: FontWeight.w500),
        ),
      ),
    );
  }

  Container buildSummaryContainer(BuildContext context, String text,
      {Color backgroundColor = MyColors.primaryColor}) {
    return Container(
      padding: const EdgeInsets.symmetric(vertical: 10, horizontal: 20),
      decoration: BoxDecoration(
        color: backgroundColor,
        borderRadius: BorderRadius.circular(30),
      ),
      child: Text(
        text,
        style: Theme.of(context)
            .textTheme
            .bodyMedium!
            .copyWith(color: Colors.white, fontWeight: FontWeight.w600),
      ),
    );
  }
}
