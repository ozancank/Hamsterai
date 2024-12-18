import 'package:flutter/material.dart';
import 'package:flutter_svg/svg.dart';
import 'package:flutter_tts/flutter_tts.dart';
import 'package:get/get.dart';
import 'package:lottie/lottie.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/date_extensions.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/module/custom_image.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/similarQuestions/model/get_similar_question_response_model.dart';
import 'package:mobile/styles/colors.dart';

class SimilarQuestionDetailPage extends StatefulWidget {
  final SimilarQuestion similarQuestion;
  const SimilarQuestionDetailPage({
    super.key,
    required this.similarQuestion,
  });

  @override
  State<SimilarQuestionDetailPage> createState() =>
      _SimilarQuestionDetailPageState();
}

class _SimilarQuestionDetailPageState extends State<SimilarQuestionDetailPage> {
  bool isAnswerVisible = false;
  final FlutterTts flutterTts = FlutterTts();
  String audioState = '';

  @override
  void initState() {
    super.initState();
  }

  Future _speak(String text) async {
    await flutterTts.setLanguage("tr-TR");
    await flutterTts.setPitch(1.0);
    await flutterTts.speak(text);
  }

  Future _stopSpeak() async {
    await flutterTts.stop();
  }

  Future _pauseSpeak() async {
    await flutterTts.pause();
  }

  @override
  void dispose() {
    _stopSpeak();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar:  CommonAppbar2(title: 'Benzer Soru Çözümü'),
      backgroundColor: const Color(0xFFF6F7F8),
      body: Stack(
        children: [
          SingleChildScrollView(
            child: Column(
              children: [
                topPart(context),
                bottomPart(context),
              ],
            ),
          ),
          widget.similarQuestion.status == 2
              ? Positioned(
                  bottom: context.height * 0.01,
                  left: context.width * 0.05,
                  child: Visibility(
                      visible: widget.similarQuestion.lessonName != 'İngilizce',
                      child: toolkit(context)),
                )
              : const SizedBox(),
        ],
      ),
    );
  }

  Container bottomPart(BuildContext context) {
    return Container(
      margin: EdgeInsets.only(
        left: context.dynamicWidth * 0.04,
        right: context.dynamicWidth * 0.04,
        bottom: context.dynamicHeight * 0.03,
        top: context.dynamicHeight * 0.01,
      ),
      padding: EdgeInsets.only(
        left: 10,
        right: 10,
        bottom: context.dynamicHeight * 0.05,
        top: 10,
      ),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            offset: const Offset(0, 24),
            blurRadius: 48,
            color: const Color(0xFFB3B8BF).withOpacity(0.7),
            spreadRadius: -16,
          ),
        ],
      ),
      child: SingleChildScrollView(
        child: Stack(
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                CustomImage(
                  isTestResult: true,
                  imageUrl:
                      '${ApplicationConstants.APIBASEURL}/SimilarQuestionPicture/${widget.similarQuestion.responseQuestionFileName}',
                  headers: ApplicationConstants.XAPIKEY,
                ),
                const SizedBox(height: 10),
                const Divider(color: MyColors.primaryColor),
                GestureDetector(
                  onTap: () {
                    setState(() {
                      isAnswerVisible = !isAnswerVisible;
                    });
                  },
                  child: Container(
                    margin: const EdgeInsets.only(top: 5),
                    padding: const EdgeInsets.all(7),
                    decoration: BoxDecoration(
                      borderRadius: BorderRadius.circular(20),
                      color: MyColors.primaryColor,
                    ),
                    child: Text(
                      isAnswerVisible ? 'Cevabı Gizle' : 'Cevabı Göster',
                      style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                            color: Colors.white,
                            fontWeight: FontWeight.w700,
                          ),
                    ),
                  ),
                ),
                const SizedBox(height: 10),
                widget.similarQuestion.status == 1
                    ? const Text('Çözüm devam ediyor...')
                    : widget.similarQuestion.status == 3
                        ? const Text(
                            'Servis tarafından hata verildi. gönderdiniz. Lütfen yeniden deneyiniz.')
                        : Visibility(
                            visible: isAnswerVisible,
                            child: CustomImage(
                              isTestResult: true,
                              imageUrl:
                                  '${ApplicationConstants.APIBASEURL}/SimilarAnswerPicture/${widget.similarQuestion.responseAnswerFileName}',
                              headers: ApplicationConstants.XAPIKEY,
                            ),
                          ),
                // : Container(
                //     padding: const EdgeInsets.all(10),
                //     width: context.dynamicWidth,
                //     decoration: BoxDecoration(
                //         color: const Color(0xFFB3B8BF),
                //         borderRadius:
                //             BorderRadius.circular(10)),
                //     child: Text(
                //         widget.similarQuestion.responseAnswer),
                //   ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Container topPart(BuildContext context) {
    return Container(
      margin: EdgeInsets.only(
        left: context.dynamicWidth * 0.04,
        right: context.dynamicWidth * 0.04,
        bottom: context.dynamicHeight * 0.03,
        top: context.dynamicHeight * 0.01,
      ),
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
        boxShadow: [
          BoxShadow(
            offset: const Offset(0, 24),
            blurRadius: 48,
            color: const Color(0xFFB3B8BF).withOpacity(0.3),
            spreadRadius: -16,
          ),
        ],
      ),
      child: Row(
        children: [
          Container(
            alignment: Alignment.center,
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: widget.similarQuestion.status == 3
                  ? Colors.red
                  : widget.similarQuestion.status == 1
                      ? const Color(0xFFD9D9D9)
                      : MyColors.primaryColor,
            ),
            child: Icon(
              widget.similarQuestion.status == 3
                  ? Icons.close
                  : widget.similarQuestion.status == 1
                      ? Icons.access_alarm_outlined
                      : Icons.done,
              color: Colors.white,
            ),
          ),
          const SizedBox(width: 15),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                widget.similarQuestion.lessonName,
                style: Theme.of(context).textTheme.titleMedium!.copyWith(
                      color: MyColors.darkBlue,
                    ),
              ),
              Text(
                widget.similarQuestion.status == 2
                    ? 'Tamamlandı - ${widget.similarQuestion.createDate.toString().toFormattedDateTime()}'
                    : widget.similarQuestion.status == 3
                        ? 'Servis tarafından hata verildi.'
                        : 'Çözüm devam ediyor..',
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.bodySmall,
              ),
            ],
          ),
        ],
      ),
    );
  }

  Align toolkit(BuildContext context) {
    return Align(
      alignment: Alignment.center,
      child: AnimatedContainer(
        duration: const Duration(seconds: 1),
        curve: Curves.easeInOut,
        margin: EdgeInsets.only(right: context.width * 0.03),
        padding: EdgeInsets.symmetric(
          vertical: context.height * 0.01,
          horizontal: context.height * 0.01,
        ),
        decoration: BoxDecoration(
          color: MyColors.primaryColor,
          borderRadius: BorderRadius.circular(30),
        ),
        child: Row(
          mainAxisSize: MainAxisSize.min,
          children: [
            GestureDetector(
              onTap: () {
                _speak(widget.similarQuestion.responseAnswer);
                setState(() {
                  audioState = 'speak';
                });
              },
              child: SvgPicture.asset(
                AssetsConstant.play,
                height: 35,
                color: Colors.white,
              ),
            ),
            const SizedBox(width: 8),
            GestureDetector(
              onTap: () {
                _pauseSpeak();
                setState(() {
                  audioState = 'pause';
                });
              },
              child: SvgPicture.asset(
                AssetsConstant.pause,
                height: 35,
              ),
            ),
            const SizedBox(width: 8),
            GestureDetector(
              onTap: () {
                _stopSpeak();
                setState(() {
                  audioState = 'stop';
                });
              },
              child: SvgPicture.asset(
                AssetsConstant.stop,
                height: 35,
              ),
            ),
            AnimatedContainer(
              duration: const Duration(seconds: 1),
              width: (audioState != 'stop' && audioState != '')
                  ? context.width * 0.1
                  : 0,
              curve: Curves.easeInOut,
              child: Lottie.asset(
                'assets/json/sound-ani.json',
                animate: audioState == 'speak',
                height: context.height * 0.04,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
