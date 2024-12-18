import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:infinite_scroll_pagination/infinite_scroll_pagination.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/extensions/date_extensions.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/core/init/cache/url_storage.dart';
import 'package:mobile/module/custom_image.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/myQuestions/controller/my_questions_controller.dart';
import 'package:mobile/pages/questionDetails/view/question_detail_page.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/pages/scan/model/get_question_model.dart';
import 'package:mobile/pages/scan/model/lesson_response_model.dart';
import 'package:mobile/styles/colors.dart';

class MyQuestionsPage extends StatefulWidget {
  const MyQuestionsPage({super.key});

  @override
  State<MyQuestionsPage> createState() => _MyQuestionsPageState();
}

class _MyQuestionsPageState extends State<MyQuestionsPage> {
  static const _pageSize = 10;
  final PagingController<int, Question> _pagingController =
      PagingController(firstPageKey: 0);
  var questionController = Get.put(MyQuestionsController());
  Lessons? selectedLesson;
  var baseUrl;
  @override
  void initState() {
    super.initState();
    getImageUrl();
    _pagingController.addPageRequestListener((pageKey) {
      _fetchPage(pageKey);
    });
  }

  Future<void> _fetchPage(int pageKey) async {
    try {
      await questionController.getQuestions(_pageSize, pageKey, hasMore: true);
      var isLastPage = questionController.questionList.length < _pageSize;

      if (questionController.hasNext.value == false) {
        _pagingController.appendLastPage(questionController.questionList);
      } else {
        final nextPageKey = pageKey + 1;
        _pagingController.appendPage(
          questionController.questionList,
          nextPageKey,
        );
      }
    } catch (error) {
      _pagingController.error = error;
    }
  }

  Future<void> refreshData() async {
    await questionController.getQuestions(_pageSize, 0);
    _pagingController.refresh();
  }

  @override
  void dispose() {
    _pagingController.dispose();
    super.dispose();
  }

  Future<String> getImageUrl() async {
    baseUrl = await UrlStorage.getBaseUrl() ?? "https://api.hamsterai.com.tr";
    return baseUrl ?? "https://api.hamsterai.com.tr";
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CommonAppbar2(
        title: 'Sorularım',
        isMyQuestion: true,
        subTitleWidget: Obx(
          () {
            if (questionController.questionResponseModel.value.count == null) {
              return const CircularProgressIndicator(color: Colors.white);
            }
            return Text(
              'Toplam ${questionController.questionResponseModel.value.count} soru gönderdiniz',
              style: Theme.of(context).textTheme.titleMedium!.copyWith(
                  color: Colors.white,
                  fontFamily: 'Quicksand',
                  fontWeight: FontWeight.w600),
            );
          },
        ),
        onTap: () {
          _showFilterDialog(context, selectedLesson);
        },
      ),
      backgroundColor: Colors.white,
      body: Container(
        color: Colors.white,
        padding: const EdgeInsets.only(top: 10),
        margin: EdgeInsets.symmetric(
          horizontal: context.dynamicWidth * 0.01,
        ),
        child: RefreshIndicator(
          color: MyColors.primaryColor,
          onRefresh: refreshData,
          child: Container(
            padding: const EdgeInsets.symmetric(horizontal: 5),
            decoration: const BoxDecoration(
              color: Colors.white,
            ),
            child: PagedListView(
              pagingController: _pagingController,
              builderDelegate: PagedChildBuilderDelegate<Question>(
                  noItemsFoundIndicatorBuilder: (context) {
                return Center(
                  child: Text(
                    'Gösterilecek soru bulunamadı',
                    style: Theme.of(context).textTheme.bodyLarge,
                  ),
                );
              }, itemBuilder: (context, item, index) {
                return InkWell(
                  onTap: () {
                    HapticFeedback.mediumImpact();
                    questionController.readQuestion(item.id);
                    Get.to(
                      () => QuestionDetailPage(
                        question: item,
                        baseUrl: baseUrl,
                      ),
                    );
                  },
                  child: Container(
                    margin: const EdgeInsets.all(5),
                    decoration: BoxDecoration(
                        color: Colors.transparent,
                        border: Border.all(color: const Color(0xFFD9D9D9)),
                        borderRadius: BorderRadius.circular(
                          10,
                        )),
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          children: [
                            // Container(
                            //   alignment: Alignment.center,
                            //   padding: const EdgeInsets.all(10),
                            //   decoration: const BoxDecoration(
                            //     shape: BoxShape.circle,
                            //     color: MyColors.primaryColor,
                            //   ),
                            //   child: QuestionIcon(
                            //     question: item,
                            //     questions: questionController.questionList,
                            //   ),
                            // ),
                            Container(
                              height: context.height * 0.07,
                              width: context.width * 0.1,
                              margin: const EdgeInsets.all(5),
                              child: Container(
                                decoration: BoxDecoration(
                                  borderRadius: BorderRadius.circular(
                                    4,
                                  ),
                                  color: Colors.black,
                                ),
                                child: CustomImage(
                                  isTestResult: true,
                                  isQuestionPage: true,
                                  imageUrl:
                                      '$baseUrl/QuestionThumbnail/${item.questionPictureFileName}',
                                  headers: ApplicationConstants.XAPIKEY,
                                ),
                              ),
                            ),
                            const SizedBox(width: 15),
                            Column(
                              crossAxisAlignment: CrossAxisAlignment.start,
                              children: [
                                Text(
                                  item.lessonName ?? '',
                                  style: Theme.of(context)
                                      .textTheme
                                      .titleMedium!
                                      .copyWith(
                                        color: MyColors.darkBlue,
                                      ),
                                ),
                                Text(
                                  (() {
                                    switch (item.status) {
                                      case 1:
                                        return 'Çözüm devam ediyor';
                                      case 2:
                                        return 'Tamamlandı - ${item.createDate.toString().toFormattedDateTime()}';
                                      case 3:
                                        return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
                                      case 4:
                                        return 'Tekrar gönderiliyor';
                                      case 5:
                                        return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
                                      case 6:
                                        return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
                                      case 7:
                                        return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
                                      case 8:
                                        return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
                                      //WaitingForOcr
                                      case 9:
                                        return 'Sorunun işlenmesi bekleniyor.';
                                      //   MustBeControlForOcr
                                      case 10:
                                        return 'Sorunun kontrol edilip, gönderilmesi bekleniyor.';
                                      // ControlledForOcr
                                      case 11:
                                        return 'Tamamlandı - ${item.createDate.toString().toFormattedDateTime()}';
                                      default:
                                        return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
                                    }
                                  })(),
                                  overflow: TextOverflow.ellipsis,
                                  style: Theme.of(context).textTheme.bodySmall,
                                ),
                              ],
                            ),
                          ],
                        ),
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
                        // Padding(
                        //   padding: const EdgeInsets.all(8.0),
                        //   child: DashedDivider(
                        //     dashWidth: 5,
                        //     dashSpace: 3,
                        //     color: (() {
                        //       switch (item.status) {
                        //         case 1:
                        //           return Colors.blue;
                        //         case 2:
                        //           return Colors.green;
                        //         case 3:
                        //           return Colors.red;
                        //         case 5:
                        //           return Colors.purple;
                        //         case 4:
                        //           return Colors.orange;
                        //         case 6:
                        //           return Colors.pink;
                        //         case 7:
                        //           return Colors.brown;
                        //         case 8:
                        //           return Colors.black;
                        //         default:
                        //           return Colors.grey.withOpacity(0.3);
                        //       }
                        //     })(),
                        //   ),
                        // ),
                      ],
                    ),
                  ),
                );
              }),
            ),
          ),
        ),
      ),
    );
  }

  void _showFilterDialog(
    BuildContext context,
    Lessons? selectedLesson,
  ) {
    final scanController = Get.put(ScanController());
    scanController.getLessons(() {
      WidgetsBinding.instance.addPostFrameCallback((_) {});
    }, context);
    showDialog(
      context: context,
      builder: (BuildContext context) {
        return AlertDialog(
          title: const Text('Lütfen Ders Seçin'),
          backgroundColor: Colors.white,
          content: Column(
            mainAxisSize: MainAxisSize.min,
            children: [
              Obx(() {
                return Column(
                  children: [
                    SizedBox(
                      width: double.infinity,
                      child: DropdownButton<Lessons>(
                        isExpanded: true,
                        dropdownColor: Colors.white,
                        hint: Text(
                          selectedLesson?.name ?? "Ders Seçiniz",
                          style: const TextStyle(fontSize: 16),
                        ),
                        items: scanController.lessons.map((Lessons lesson) {
                          return DropdownMenuItem<Lessons>(
                            value: lesson,
                            child: Text(lesson.name),
                          );
                        }).toList(),
                        onChanged: (Lessons? newLesson) async {
                          if (newLesson != null) {
                            selectedLesson = newLesson;
                            int selectedLessonId = newLesson.id;
                            if (selectedLessonId != 0) {
                              questionController.lessonId.value =
                                  selectedLessonId;
                              await questionController.getQuestions(
                                  _pageSize, 0);
                              _pagingController.refresh();
                              Navigator.of(context).pop();
                              questionController.lessonId.value = 0;
                            }
                            print(
                                'Seçilen Ders: ${newLesson.name}, ID: ${newLesson.id}');
                          }
                        },
                      ),
                    ),
                  ],
                );
              }),
            ],
          ),
          actions: [
            TextButton(
              onPressed: () {
                Navigator.of(context).pop();
              },
              child: Text(
                'İptal',
                style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                    color: MyColors.primaryColor, fontWeight: FontWeight.w600),
              ),
            ),
            TextButton(
              onPressed: () async {
                await questionController.getQuestions(_pageSize, 0);
                _pagingController.refresh();
                Get.back();
              },
              child: Text(
                'Seçimi Temizle',
                style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                    color: MyColors.primaryColor, fontWeight: FontWeight.w600),
              ),
            ),
          ],
        );
      },
    );
  }
}
