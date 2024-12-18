import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/date_extensions.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/common/dash_divider.dart';
import 'package:mobile/pages/similarQuestions/controller/similar_question_controller.dart';
import 'package:mobile/pages/similarQuestions/view/similar_question_detail_page.dart';
import 'package:mobile/styles/colors.dart';

class SimilarQuestionPage extends StatefulWidget {
  const SimilarQuestionPage({super.key});

  @override
  State<SimilarQuestionPage> createState() => _SimilarQuestionPageState();
}

class _SimilarQuestionPageState extends State<SimilarQuestionPage> {
  var similarQuestionController = Get.put(SimilarQuestionController());
  bool isScrolled = false;

  Future<void> refreshData() async {
    similarQuestionController.getSimilarQuestions();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar:  CommonAppbar2(title: 'Benzer Sorularım'),
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
          // child: ShaderMask(
          //   shaderCallback: (Rect bounds) {
          //     return LinearGradient(
          //       end: Alignment.topCenter,
          //       begin: Alignment.center,
          //       colors: [
          //         Colors.white,
          //         isScrolled ? Colors.white.withOpacity(0.05) : Colors.white
          //       ],
          //       stops: const [0.9, 1],
          //       tileMode: TileMode.mirror,
          //     ).createShader(bounds);
          //   },
          child: Container(
              padding: const EdgeInsets.symmetric(horizontal: 10),
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
                if (similarQuestionController.loading.value) {
                  return const Center(
                    child: CircularProgressIndicator(
                      color: MyColors.primaryColor,
                    ),
                  );
                }
                var questions = similarQuestionController.questionList;
                if (questions.isEmpty) {
                  return Center(
                    child: Text(
                      'Gösterilecek soru bulunamadı',
                      style: Theme.of(context).textTheme.bodyLarge,
                    ),
                  );
                }
                // return NotificationListener(
                //   onNotification: (scrollNotification) {
                //     if (scrollNotification is ScrollUpdateNotification) {
                //       if (scrollNotification.metrics.pixels > 0) {
                //         setState(() {
                //           isScrolled = true;
                //         });
                //       }
                //     } else if (scrollNotification is ScrollEndNotification) {
                //       if (scrollNotification.metrics.pixels <= 0) {
                //         setState(() {
                //           isScrolled = false;
                //         });
                //       }
                //     }
                //     return true;
                //   },
                //   child:
                return ListView.builder(
                  itemCount: similarQuestionController.questionList.length,
                  itemBuilder: (context, index) {
                    return GestureDetector(
                      onTap: () {
                        HapticFeedback.mediumImpact();
                        Get.to(() => SimilarQuestionDetailPage(
                              similarQuestion: questions[index],
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
                                  crossAxisAlignment: CrossAxisAlignment.start,
                                  children: [
                                    Text(
                                      similarQuestionController
                                          .questionList[index].lessonName,
                                      style: Theme.of(context)
                                          .textTheme
                                          .titleMedium!
                                          .copyWith(
                                            color: MyColors.darkBlue,
                                          ),
                                    ),
                                    Text(
                                      similarQuestionController
                                                  .questionList[index].status ==
                                              2
                                          ? 'Tamamlandı - ${similarQuestionController.questionList[index].createDate.toString().toFormattedDateTime()}'
                                          : similarQuestionController
                                                      .questionList[index]
                                                      .status ==
                                                  3
                                              ? 'Servis tarafından hata verildi.'
                                              : 'Çözüm devam ediyor..',
                                      overflow: TextOverflow.ellipsis,
                                      style:
                                          Theme.of(context).textTheme.bodySmall,
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
                  },
                );
              })),
        ),
      ),
    );
  }
}
