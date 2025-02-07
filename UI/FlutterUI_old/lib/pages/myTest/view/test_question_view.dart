import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/module/custom_image.dart';
import 'package:mobile/pages/common/common_bottom_sheet.dart';
import 'package:mobile/pages/myTest/controller/quiz_controller.dart';
import 'package:mobile/pages/myTest/model/quiz_list_model.dart';
import 'package:mobile/pages/myTest/view/common/drawing_painter.dart';
import 'package:mobile/pages/myTest/view/common/test_appbar.dart';
import 'package:mobile/pages/myTest/view/test_result_view.dart';
import 'package:mobile/styles/colors.dart';

class TestQuestionView extends StatefulWidget {
  final QuizItem quizItem;
  final bool fromResultPage;
  const TestQuestionView(
      {super.key, required this.quizItem, this.fromResultPage = false});

  @override
  State<TestQuestionView> createState() => _TestQuestionViewState();
}

class _TestQuestionViewState extends State<TestQuestionView> {
  final quizController = Get.put(QuizController());
  List<Offset?> points = [];
  Color selectedColor = Colors.black;
  double strokeWidth = 2.0;

  bool isDrawingMode = false;
  String? _selectedItem;
  int currentQuestionIndex = 0;
  @override
  void initState() {
    quizController.loadAnswers(widget.quizItem.id);
    loadQuizData();
    super.initState();
    print(quizController.selectedAnswers);
  }

  void loadQuizData() {
    WidgetsBinding.instance.addPostFrameCallback((_) async {
      _selectedItem = quizController.getSelectedAnswer(currentQuestionIndex);
      setState(() {});
    });
  }

  void onOptionSelected(int questionNumber, String selectedOption) {
    quizController.saveAnswer(
        quizController.quizModel.value!.id, questionNumber, selectedOption);
  }

  void clearCanvas() {
    setState(() {
      points = [];
    });
  }

  void toggleDrawingMode() {
    setState(() {
      isDrawingMode = true;
    });
  }

  void toggleUnDrawingMode() {
    setState(() {
      isDrawingMode = false;
    });
  }

  Future<void> nextQuestion() async {
    setState(() {
      if (currentQuestionIndex < quizController.quizQuestionList.length - 1) {
        if (_selectedItem != null) {
          quizController.saveAnswer(
              widget.quizItem.id, currentQuestionIndex, _selectedItem!);
        }
        currentQuestionIndex = currentQuestionIndex + 1;
        _selectedItem = quizController.getSelectedAnswer(currentQuestionIndex);
        print('Kaydedilen Cevap: $currentQuestionIndex -> $_selectedItem');
        clearCanvas();
      } else {
        showModalBottomSheet(
          isDismissible: false,
          enableDrag: false,
          shape: showModalSheetShape(),
          backgroundColor: Colors.white,
          isScrollControlled: true,
          context: context,
          builder: (context) {
            return WillPopScope(
              onWillPop: () async {
                return false;
              },
              child: CommonBottomSheet(
                approveButtonColor: MyColors.greenColor,
                approveText: 'Testi Tamamla',
                approveOnTap: () async {
                  await quizController.updateQuiz(widget.quizItem.id);
                  Get.to(() => const TestResultView());
                },
                cancelButtonColor: Colors.white,
                cancelText: 'Geri Dön',
                cancelTextColor: MyColors.primaryColor,
                headerText: 'Emin misin?',
                smallText: 'Testi tamamlamak üzeresin!',
              ),
            );
          },
        );
      }
    });
  }

  void previousQuestion() {
    setState(() {
      if (currentQuestionIndex > 0) {
        if (_selectedItem != null) {
          quizController.saveAnswer(
              widget.quizItem.id, currentQuestionIndex, _selectedItem!);
        }
        currentQuestionIndex = currentQuestionIndex - 1;
        _selectedItem = quizController.getSelectedAnswer(currentQuestionIndex);
      }
    });
  }

  @override
  Widget build(BuildContext context) {
    return PopScope(
      canPop: false,
      child: Scaffold(
        backgroundColor: Colors.white,
        appBar: TestAppbar(
          questionIndex: currentQuestionIndex + 1,
          quizLength: quizController.quizQuestionList.length,
          quizItem: widget.quizItem,
        ),
        body: GestureDetector(
          onPanUpdate: isDrawingMode
              ? (details) {
                  setState(() {
                    if (quizController.isEraserMode.value) {
                      points = points.where((point) {
                        if (point == null) return true;
                        return (details.localPosition - point).distance >
                            strokeWidth;
                      }).toList();
                    } else {
                      points.add(details.localPosition);
                    }
                  });
                }
              : null,
          onPanEnd: isDrawingMode
              ? (details) {
                  setState(() {
                    if (!quizController.isEraserMode.value) {
                      points.add(null);
                    }
                  });
                }
              : null,
          child: Stack(
            children: [
              Obx(() {
                if (quizController.loading.value) {
                  return const Center(child: CircularProgressIndicator());
                }
                return Align(
                  alignment: Alignment.topCenter,
                  child: CustomImage(
                    isTestResult: true,
                    key: ValueKey(currentQuestionIndex),
                    imageUrl:
                        '${ApplicationConstants.APIBASEURL}/QuizQuestionPicture/${quizController.quizQuestionList[currentQuestionIndex].questionPictureFileName}',
                    headers: ApplicationConstants.XAPIKEY,
                  ),
                );
              }),
              RepaintBoundary(
                child: CustomPaint(
                  painter: DrawingPainter(points, selectedColor, strokeWidth,
                      quizController.isEraserMode.value),
                  child: Container(
                    width: double.infinity,
                    height: double.infinity,
                    color: Colors.transparent,
                  ),
                ),
              ),
              toolkit(context),
            ],
          ),
        ),
        bottomNavigationBar: BottomAppBar(
          color: Colors.transparent,
          child: Row(
            mainAxisAlignment: MainAxisAlignment.spaceAround,
            children: [
              GestureDetector(
                  onTap: previousQuestion,
                  child: SvgPicture.asset(AssetsConstant.left)),
              Obx(() {
                if (quizController.loading.value) {
                  return const SizedBox.shrink();
                }
                return Row(
                  mainAxisAlignment: MainAxisAlignment.spaceEvenly,
                  children: List.generate(
                    quizController
                        .quizQuestionList[currentQuestionIndex].optionCount
                        .clamp(0, 5),
                    (optionIndex) {
                      String label = String.fromCharCode(65 + optionIndex);
                      return _buildSelectableContainer(
                      label,
                        currentQuestionIndex,
                        quizController.quizModel.value!.id,
                      );
                    },
                  ),
                );
              }),
              GestureDetector(
                onTap: nextQuestion,
                child: SvgPicture.asset(AssetsConstant.right),
              ),
            ],
          ),
        ),
      ),
    );
  }

  Widget _buildSelectableContainer(
      String label, int questionNumber, String quizId) {
    bool isSelected = _selectedItem == label;
    return Padding(
      padding: EdgeInsets.symmetric(horizontal: context.width * 0.01),
      child: GestureDetector(
        onTap: () {
          setState(() {
            if (_selectedItem == label) {
              _selectedItem = null;
            } else {
              _selectedItem = label;
              onOptionSelected(questionNumber, label);
            }
          });
        },
        child: Container(
          padding: const EdgeInsets.all(10),
          decoration: BoxDecoration(
            shape: BoxShape.circle,
            color: isSelected ? Colors.black : const Color(0xFFF6F7F8),
            border: Border.all(color: const Color(0xFFD9D9D9)),
          ),
          child: Center(
            child: Text(
              label,
              style: Theme.of(context)
                  .textTheme
                  .bodyLarge!
                  .copyWith(color: isSelected ? Colors.white : Colors.black),
            ),
          ),
        ),
      ),
    );
  }

  Align toolkit(BuildContext context) {
    return Align(
      alignment: Alignment.centerRight,
      child: Container(
        margin: EdgeInsets.only(right: context.width * 0.03),
        padding: EdgeInsets.symmetric(
            vertical: context.height * 0.03, horizontal: 5),
        decoration: BoxDecoration(
          color: Colors.white,
          borderRadius: BorderRadius.circular(20),
          border: Border.all(color: Colors.grey),
        ),
        child: Column(
          mainAxisSize: MainAxisSize.min,
          children: [
            GestureDetector(
              onTap: toggleUnDrawingMode,
              child: SvgPicture.asset(
                AssetsConstant.touch,
                color: isDrawingMode
                    ? const Color(0xFF808080)
                    : MyColors.primaryColor,
                height: 30,
              ),
            ),
            const SizedBox(height: 8),
            GestureDetector(
              onTap: toggleDrawingMode,
              child: SvgPicture.asset(
                color: isDrawingMode
                    ? MyColors.primaryColor
                    : const Color(0xFF808080),
                AssetsConstant.pencil,
                height: 30,
              ),
            ),
            const SizedBox(height: 8),
            GestureDetector(
              onTap: clearCanvas,
              child: SvgPicture.asset(
                AssetsConstant.delete,
                height: 30,
              ),
            ),
          ],
        ),
      ),
    );
  }

 

  RoundedRectangleBorder showModalSheetShape() {
    return const RoundedRectangleBorder(
      borderRadius: BorderRadius.only(
        topRight: Radius.circular(24),
        topLeft: Radius.circular(24),
      ),
    );
  }
}
