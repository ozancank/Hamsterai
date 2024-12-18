import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/svg.dart';
import 'package:flutter_tts/flutter_tts.dart';
import 'package:get/get.dart';
import 'package:lottie/lottie.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/date_extensions.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/core/init/cache/url_storage.dart';
import 'package:mobile/module/custom_image.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:mobile/pages/common/common_bottom_sheet.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/pages/questionDetails/controller/question_detail_controller.dart';
import 'package:mobile/pages/scan/model/get_question_model.dart';
import 'package:mobile/styles/colors.dart';
import 'package:webview_flutter/webview_flutter.dart';

class QuestionDetailPage extends StatefulWidget {
  final Question question;
  final baseUrl;
  const QuestionDetailPage(
      {super.key, required this.question, required this.baseUrl});

  @override
  State<QuestionDetailPage> createState() => _QuestionDetailPageState();
}

class _QuestionDetailPageState extends State<QuestionDetailPage> {
  late final WebViewController _controller;
  final questionDetailController = Get.put(QuestionDetailController());
  double _webViewHeight = 200;
  final FlutterTts flutterTts = FlutterTts();
  String audioState = '';
  double normalSpeed = 0.557;
  double speakSpeed = 0.557;
  bool sendAgain = true;
  String? _currentText;
  int _currentOffset = 0;
  String updatedHtmlContent = '';
  var baseUrl;

  String latex = '''


Soruda verilen eşitsizlik:

\$\frac{x^2(x+m)}{(x+n)(2mx^2-n)} \le 0\$

\$m < 0 < n\$  bilgisi verilmiş.  \$x^2 \ge 0\$ olduğu için eşitsizliği etkilemez.  Paydadaki \$(x+n)\$ ifadesi \$n>0\$ olduğundan \$x>-n\$ için pozitif, \$x<-n\$ için negatiftir.  Paydaki \$(x+m)\$ ifadesi \$m<0\$ olduğundan \$x>-m\$ için pozitif, \$x<-m\$ için negatiftir.

Paydadaki \$2mx^2 - n = 0\$ denklemini çözelim: \$2mx^2 = n \implies x^2 = \frac{n}{-2m}\$.  \$n>0\$ ve \$m<0\$ olduğundan \$\frac{n}{-2m} > 0\$ olur.  Bu durumda \$x = \pm \sqrt{\frac{n}{-2m}}\$ değerleri için payda sıfırdır.

Şimdi eşitsizliğin işaretini inceleyelim:

\begin{itemize}
    \item \$x<-n\$: \$(x+n)<0\$, \$(x+m)\$ pozitif veya negatif olabilir, payda negatiftir.  Eşitsizliğin sağlanması için \$(x+m) \le 0\$ olmalıdır, yani \$x \le -m\$.  Bu durumda \$x<-n\$ aralığı için çözüm kümesi \$x \le -m\$ yani  \$x \in (-\infty, -n) \cup (-n,-m]\$ olur.
    \item \$-n < x < 0\$: \$(x+n)>0\$, \$(x+m)\$ pozitif veya negatif olabilir, payda pozitiftir.  Eşitsizliğin sağlanması için \$(x+m) \le 0\$ olmalıdır, yani \$x \le -m\$. Bu durumda \$x \in [-n, -m]\$ olur.
    \item \$x=0\ Eşitsizlik sağlanır.
    \item \$0 < x\$: \$(x+n)>0\$, \$(x+m)>0\$, payda pozitiftir. Eşitsizlik sağlanmaz.
    \item \$x > -m\$: \$(x+n)>0\$, \$(x+m)>0\$, payda pozitiftir.  Eşitsizlik sağlanmaz.
\end{itemize}

Tüm aralıkları birleştirdiğimizde çözüm kümesi \$[-n, -m] \cup \{0\}\$ olur.


Doğru Cevap) B''';
  bool isLoading = true;
  Future _speak(String text) async {
    _currentText = text;
    _currentOffset = 0;
    await flutterTts.setLanguage("tr-TR");
    List<dynamic> voices = await flutterTts.getVoices;
    List<Map<String, String>> selectedVoice = voices
        .map((voice) => Map<String, String>.from(voice))
        .where((x) => x["locale"] == 'tr-TR')
        .toList();

    selectedVoice.sort((a, b) => a["name"]!.compareTo(b["name"]!));
    if (selectedVoice.length > 5) {
      await flutterTts.setVoice(selectedVoice[5]);
    } else {
      await flutterTts.setVoice(selectedVoice.first);
    }
    await flutterTts.setPitch(0.8);
    await flutterTts.setSpeechRate(speakSpeed);

    await flutterTts.speak(text);

    flutterTts
        .setProgressHandler((String text, int start, int end, String word) {
      _currentOffset = end;
    });
  }

  Future<void> _resumeSpeak() async {
    if (_currentText != null && _currentOffset < _currentText!.length) {
      String remainingText = _currentText!.substring(_currentOffset);
      await flutterTts.setSpeechRate(speakSpeed);
      await flutterTts.speak(remainingText);
    }
  }

  Future _stopSpeak() async {
    await flutterTts.stop();
  }

  Future _pauseSpeak() async {
    await flutterTts.pause();
  }

  Future<String> getImageUrl() async {
    baseUrl = await UrlStorage.getBaseUrl() ?? "https://api.hamsterai.com.tr";
    return baseUrl ?? "https://api.hamsterai.com.tr";
  }

  @override
  void initState() {
    super.initState();

    _controller = WebViewController()
      ..setJavaScriptMode(JavaScriptMode.unrestricted)
      ..addJavaScriptChannel(
        'contentHeight',
        onMessageReceived: (message) {
          setState(() {
            _webViewHeight = double.tryParse(message.message) ?? 100;
          });
        },
      )
      ..addJavaScriptChannel(
        'updatedContentChannel',
        onMessageReceived: (message) {
          updatedHtmlContent = message.message;
          print('Updated Content: $updatedHtmlContent');
        },
      )
      ..setNavigationDelegate(
        NavigationDelegate(
          onPageStarted: (url) {
            setState(() {
              isLoading = true;
            });
          },
          onPageFinished: (url) {
            setState(() {
              isLoading = false;
            });
          },
        ),
      )
      ..loadHtmlString(generateHtmlContent());
    getImageUrl();
    print(widget.question.answerText!.replaceAll('\\n', '<br>'));
  }

  String generateHtmlContent() {
    return '''
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>Matematik Sonuç</title>
  <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.0/css/bootstrap.min.css">
  <link href="https://fonts.googleapis.com/css2?family=Quicksand:wght@400;700&display=swap" rel="stylesheet">

  
  <style>
    body {
      overflow-x: scroll;
      white-space: nowrap;
      font-family: 'Quicksand', sans-serif;
      font-size: 2.5rem !important;
      color: #000000;
      background-color: transparent;
    }
    #mathContent {
      white-space: pre-wrap;
      overflow-y: scroll;
    }
    #mathContent > span{
       margin-top:30px;
      }
    .mjx-chtml{
      line-height:1.7rem;
    }
    #mathContent .alert{
        margin-bottom:0;
        width:100%;
        display:block;
      }
  </style>
   <script>
         function sendHeightToFlutter() {
  if (window.contentHeight) {
    const height = document.body.scrollHeight+300;
    window.contentHeight.postMessage(height.toString());

  } else {
    console.error('JavaScriptChannel "contentHeight" bulunamadı.');
  }
}

// Sayfa yüklenince tetikleme
window.addEventListener('load', function() {
  sendHeightToFlutter();
});
          
        </script>
  <script type="text/javascript">
    window.addEventListener('load', function() {
      if (typeof MathJax !== 'undefined') {
        MathJax.Hub.Register.StartupHook('End', function() {
          console.log('MathJax Yüklendi');
        });
      }
    });

    function addText(text) {
      var mathContent = document.getElementById("mathContent");
      mathContent.innerHTML = text;
      MathJax.Hub.Queue(["Typeset", MathJax.Hub, mathContent]);
    }
  </script>
  <script type="text/x-mathjax-config">
  const size = screen.width;
  if(size > 768){
    MathJax.Hub.Config({
      jax: ["input/TeX", "output/HTML-CSS"],
      extensions: ["tex2jax.js", "asciimath2jax.js", "MathMenu.js", "MathZoom.js", "AssistiveMML.js", "a11y/accessibility-menu.js"],
      TeX: {
        extensions: ["AMSmath.js", "AMSsymbols.js", "noErrors.js", "noUndefined.js"]
      },
      tex2jax: {
    inlineMath: [['\$','\$']],
    processEscapes: true
  },
 CommonHTML: {
            scale: 90
        },
        "HTML-CSS": {
            scale: 90
        },
        NativeMML: {
            scale: 90
        },
        SVG: {
            scale: 90
        },
        PreviewHTML: {
            scale: 90
        },
    });
  }else{
    MathJax.Hub.Config({
      jax: ["input/TeX", "output/HTML-CSS"],
      extensions: ["tex2jax.js", "asciimath2jax.js", "MathMenu.js", "MathZoom.js", "AssistiveMML.js", "a11y/accessibility-menu.js"],
      TeX: {
        extensions: ["AMSmath.js", "AMSsymbols.js", "noErrors.js", "noUndefined.js"]
      },
      tex2jax: {
    inlineMath: [['\$','\$']],
    processEscapes: true
  },
 CommonHTML: {
            scale: 120
        },
        "HTML-CSS": {
            scale: 120
        },
        NativeMML: {
            scale: 120
        },
        SVG: {
            scale: 120
        },
        PreviewHTML: {
            scale: 120
        },
    });
  }
    
  </script>
  <script async src="https://cdnjs.cloudflare.com/ajax/libs/mathjax/2.7.7/MathJax.js?config=TeX-MML-AM_CHTML"></script>
  <script> 
    window.onload= function(){
      const brs = document.querySelectorAll(".alert>br");
      for(const br of brs){
        br.remove();
      }
    }
  </script>

  <script>
  // Flutter'a mesaj göndermek için kanal oluşturuyoruz
  function sendUpdatedContentToFlutter() {
    if (window.updatedContentChannel) {
      const updatedContent = document.getElementById('mathContent').innerHTML;
      window.updatedContentChannel.postMessage(updatedContent);
    } else {
      console.error('JavaScriptChannel "updatedContentChannel" bulunamadı.');
    }
  }

  // İçerik düzenlenince değişiklikleri algılamak için event listener ekliyoruz
  document.addEventListener('DOMContentLoaded', function () {
    const mathContent = document.getElementById('mathContent');
    mathContent.addEventListener('input', sendUpdatedContentToFlutter);
  });
</script>
</head>
<body>
  <div id="mathContent" contenteditable= "${widget.question.status == 10 ? 'true' : 'false'}">${widget.question.status == 10 ? widget.question.questionText?.replaceAll('\\n', '<br>').replaceAll('<', ' < ').replaceAll('>', ' > ').replaceAll('\\begin{itemize}', '').replaceAll('\\item', '<br>').replaceAll('\\end{itemize}', '') : widget.question.answerText?.replaceAll('\\n', '<br>').replaceAll('<', ' < ').replaceAll('>', ' > ').replaceAll('\\begin{itemize}', '').replaceAll('\\item', '<br>').replaceAll('\\end{itemize}', '')}</div> 
  
</body>
</html>
    '''
        .trim();
  }

  @override
  void dispose() {
    _stopSpeak();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CommonAppbar2(
        title: widget.question.lessonName ?? '',
        isDetail: true,
        subTitle: (() {
          switch (widget.question.status) {
            case 1:
              return 'Çözüm devam ediyor...';
            case 2:
              return 'Tamamlandı - ${widget.question.createDate.toString().toFormattedDateTime()}';
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
              return 'Tamamlandı - ${widget.question.createDate.toString().toFormattedDateTime()}';
            default:
              return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
          }
        })(),
        widget: Visibility(
          visible: !widget.question.manuelSendAgain! &&
              widget.question.status != 9 &&
              widget.question.status != 10 &&
              widget.question.status != 11,
          child: Padding(
            padding: const EdgeInsets.only(right: 10),
            child: IconButton(
              onPressed: () async {
                HapticFeedback.mediumImpact();
                await showModalBottomSheet(
                  shape: showModalSheetShape(),
                  backgroundColor: Colors.white,
                  isScrollControlled: true,
                  context: context,
                  builder: (context) {
                    return CommonBottomSheet(
                      approveText: 'Gönder',
                      headerText: 'Soruyu tekrar göndermek istiyor musunuz?',
                      approveOnTap: () async {
                        setState(() {
                          sendAgain = false;
                        });
                        await questionDetailController
                            .reSendQuestion(widget.question.id);
                        Get.offAllNamed('/home');
                      },
                      smallText:
                          'Soruyu tekrar göndermeniz, soru gönderim hakkınızdan düşecektir.',
                    );
                  },
                );
              },
              icon: const Icon(
                Icons.refresh_sharp,
                color: Colors.white,
                size: 30,
              ),
            ),
          ),
        ),
      ),
      backgroundColor: Colors.white,
      body: Stack(
        children: [
          SingleChildScrollView(
            child: Column(
              children: [
                bottomPart(context),
              ],
            ),
          ),
          widget.question.status == 1
              ? Positioned(
                  bottom: context.height * 0.02,
                  left: context.height * 0.02,
                  child: Visibility(
                    visible: true,
                    child: widget.question.lessonType == 1
                        ? const SizedBox()
                        : toolkit(context),
                  ),
                )
              : widget.question.status == 10
                  ? Container(
                      margin: EdgeInsets.only(
                        top: context.height * 0.70,
                        bottom: context.height * 0.05,
                        left: context.width * 0.1,
                        right: context.width * 0.1,
                      ),
                      child: CommonButton(
                        textColor: Colors.white,
                        bgcolor: MyColors.primaryColor,
                        text: 'Soruyu gönder',
                        fontSize: 20,
                        onTap: () async {
                          questionDetailController.updateQuestionText(
                              widget.question.id, updatedHtmlContent);
                        },
                      ),
                    )
                  : const SizedBox()
        ],
      ),
    );
  }

  bottomPart(BuildContext context) {
    return Container(
      margin: const EdgeInsets.only(),
      padding: EdgeInsets.only(
        left: 10,
        right: 10,
        bottom: context.dynamicHeight * 0.05,
        top: 10,
      ),
      decoration: BoxDecoration(
        color: Colors.white,
        borderRadius: BorderRadius.circular(20),
      ),
      child: SingleChildScrollView(
        child: Stack(
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.center,
              children: [
                Padding(
                  padding: EdgeInsets.only(left: context.width * 0.35),
                  child: CustomImage(
                    imageUrl:
                        '${widget.baseUrl}/QuestionPicture/${widget.question.questionPictureFileName}',
                    headers: ApplicationConstants.XAPIKEY,
                  ),
                ),
                const SizedBox(height: 10),
                // widget.question.status == 1
                //     ? const Text('Çözüm devam ediyor...')
                //     : widget.question.status == 3
                //         ? const Text(
                //             'Lütfen soruyu daha net şekilde tekrar gönderiniz.')
                //         : widget.question.status == 4
                //             ? const Text('Tekrar gönderiliyor')
                //             : widget.question.status == 5
                //                 ? const Text('Servis tarafından hata verildi')
                //                 : widget.question.status == 6
                //                     ? const Text('Bağlantı hatası')
                //                     : widget.question.status == 7
                //                         ? const Text('Zaman aşımı')
                //                         : widget.question.status == 8
                //                             ? const Text('Resim bulunamadı')
                //                             : Stack(
                //                                 children: [
                //                                   Container(
                //                                     padding:
                //                                         const EdgeInsets.all(0),
                //                                     width: context.dynamicWidth,
                //                                     decoration: BoxDecoration(
                //                                       color: Colors.transparent,
                //                                       borderRadius:
                //                                           BorderRadius.circular(
                //                                               20),
                //                                     ),
                //                                     child: SizedBox(
                //                                       height: _webViewHeight,
                //                                       width: double.infinity,
                //                                       child: WebViewWidget(
                //                                         controller: _controller,
                //                                       ),
                //                                     ),
                //                                   ),
                //                                   if (isLoading)
                //                                     const Center(
                //                                       child:
                //                                           CircularProgressIndicator(),
                //                                     ),
                //                                 ],
                //                               ),

                buildStatusText(),

                Align(
                  alignment: Alignment.bottomRight,
                  child: Container(
                    margin: const EdgeInsets.only(top: 20, right: 40),
                    height: 3,
                    width: 3,
                    decoration: BoxDecoration(
                        shape: BoxShape.circle,
                        color: (() {
                          switch (widget.question.ocrMethod) {
                            case "gemini":
                              return Colors.blue;
                            case "tesseract":
                              return Colors.red;
                            default:
                              return Colors.grey.withOpacity(0.3);
                          }
                        })()),
                  ),
                )
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget buildStatusText() {
    switch (widget.question.status) {
      case 1:
        return const Text('Çözüm devam ediyor...');
      case 3:
        return const Text('Lütfen soruyu daha net şekilde tekrar gönderiniz.');
      case 4:
        return const Text('Tekrar gönderiliyor');
      case 5:
        return const Text('Servis tarafından hata verildi');
      case 6:
        return const Text('Bağlantı hatası');
      case 7:
        return const Text('Zaman aşımı');
      case 8:
        return const Text('Resim bulunamadı');
      default:
        return Stack(
          children: [
            Container(
              padding: const EdgeInsets.all(0),
              width: context.dynamicWidth,
              decoration: BoxDecoration(
                color: Colors.transparent,
                borderRadius: BorderRadius.circular(20),
              ),
              child: SizedBox(
                height: _webViewHeight,
                width: double.infinity,
                child: WebViewWidget(
                  controller: _controller,
                ),
              ),
            ),
            if (isLoading)
              const Center(
                child: CircularProgressIndicator(),
              ),
          ],
        );
    }
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
                _speak(widget.question.answerText ?? '');
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
                  ? context.width * 0.22
                  : 0,
              curve: Curves.easeInOut,
              child: Visibility(
                visible: audioState != 'stop' && audioState != '',
                child: SingleChildScrollView(
                  scrollDirection: Axis.horizontal,
                  child: Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      const SizedBox(width: 8),
                      Lottie.asset(
                        'assets/json/sound-ani.json',
                        animate: audioState == 'speak',
                        height: context.height * 0.04,
                      ),
                      const SizedBox(width: 8),
                      GestureDetector(
                        onTap: () async {
                          await flutterTts.stop();
                          setState(() {
                            speakSpeed =
                                speakSpeed == normalSpeed ? 0.7 : normalSpeed;
                          });
                          await _resumeSpeak();
                        },
                        child: Container(
                          width: 30,
                          height: 30,
                          decoration: BoxDecoration(
                            shape: BoxShape.circle,
                            border: Border.all(
                              color: Colors.white,
                              width: 2,
                            ),
                          ),
                          child: Center(
                            child: Text(
                              speakSpeed == normalSpeed ? '1X' : "2X",
                              style: Theme.of(context)
                                  .textTheme
                                  .bodyMedium!
                                  .copyWith(
                                    color: Colors.white,
                                    fontWeight: FontWeight.bold,
                                    fontFamily: "Manrope",
                                    fontSize: 15,
                                  ),
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ),
              ),
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
        border: Border.all(
          width: 1,
          color: (() {
            switch (widget.question.status) {
              case 1:
                return Colors.blue;
              case 2:
                return Colors.green;
              case 3:
                return Colors.red;
              case 5:
                return Colors.purple;
              case 4:
                return Colors.orange;
              case 6:
                return Colors.pink;
              case 7:
                return Colors.brown;
              case 8:
                return Colors.black;
              default:
                return Colors.grey.withOpacity(0.3);
            }
          })(),
        ),
      ),
      child: Row(
        children: [
          Container(
            alignment: Alignment.center,
            padding: const EdgeInsets.all(10),
            decoration: BoxDecoration(
              shape: BoxShape.circle,
              color: widget.question.status == 3
                  ? Colors.red
                  : widget.question.status == 1
                      ? const Color(0xFFD9D9D9)
                      : MyColors.primaryColor,
            ),
            child: Icon(
              (() {
                switch (widget.question.status) {
                  case 1:
                    return Icons.hourglass_bottom_sharp;
                  case 2:
                    return Icons.done;
                  case 3:
                    return Icons.close;
                  case 5:
                    return Icons.hourglass_bottom_sharp;
                  case 4:
                    return Icons.hourglass_bottom_sharp;
                  case 6:
                    return Icons.hourglass_bottom_sharp;
                  case 7:
                    return Icons.close;
                  case 8:
                    return Icons.hourglass_bottom_sharp;
                  default:
                    return Icons.hourglass_bottom_sharp;
                }
              })(),
              color: Colors.white,
            ),
          ),
          const SizedBox(width: 15),
          Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Text(
                widget.question.lessonName ?? '',
                style: Theme.of(context).textTheme.titleMedium!.copyWith(
                      color: MyColors.darkBlue,
                    ),
              ),
              Text(
                (() {
                  switch (widget.question.status) {
                    case 1:
                      return 'Çözüm devam ediyor...';
                    case 2:
                      return 'Tamamlandı - ${widget.question.createDate.toString().toFormattedDateTime()}';
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
                    default:
                      return 'Lütfen soruyu daha net şekilde tekrar gönderiniz.';
                  }
                })(),
                overflow: TextOverflow.ellipsis,
                style: Theme.of(context).textTheme.bodySmall,
              ),
            ],
          ),
          const Spacer(),
        ],
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
