import 'dart:typed_data';

import 'package:dio/dio.dart';
import 'package:flutter/material.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/styles/colors.dart';

class CustomImage extends StatefulWidget {
  final String imageUrl;
  final Map<String, String> headers;
  final bool isTouchable;
  final bool isTestResult;
  final bool isQuestionPage;

  const CustomImage({
    required this.imageUrl,
    required this.headers,
    this.isTouchable = true,
    this.isTestResult = false,
    super.key,
    this.isQuestionPage = false,
  });

  @override
  CustomImageState createState() => CustomImageState();
}

class CustomImageState extends State<CustomImage> {
  late Future<Image> _imageFuture;

  @override
  void initState() {
    super.initState();
    _imageFuture = _loadImage();
  }

  Future<Image> _loadImage() async {
    try {
      final dio = Dio();
      final response = await dio.get<ResponseBody>(
        widget.imageUrl,
        options: Options(
          headers: widget.headers,
          responseType: ResponseType.stream,
        ),
      );

      final bytes = await _readStream(response.data!.stream);
      return Image.memory(bytes);
    } catch (e) {
      print('Error loading image: $e');
      return Image.asset(AssetsConstant.placeHolder);
    }
  }

  Future<Uint8List> _readStream(Stream<List<int>> stream) async {
    final bytesBuilder = BytesBuilder();
    await for (final chunk in stream) {
      bytesBuilder.add(chunk);
    }
    return bytesBuilder.toBytes();
  }

  @override
  Widget build(BuildContext context) {
    print(widget.imageUrl);
    return FutureBuilder<Image>(
      future: _imageFuture,
      builder: (BuildContext context, AsyncSnapshot<Image> snapshot) {
        if (snapshot.connectionState == ConnectionState.waiting) {
          return const Center(
            child: CircularProgressIndicator(color: MyColors.primaryColor),
          );
        } else if (snapshot.hasError) {
          return const Center(
            child: Text('Error loading image'),
          );
        } else {
          return ClipRRect(
            borderRadius: widget.isTestResult
                ? BorderRadius.circular(0)
                : BorderRadius.circular(0),
            child: GestureDetector(
              onTap: widget.isTouchable
                  ? () {
                      showDialog(
                        context: context,
                        builder: (BuildContext context) {
                          return Dialog(
                            backgroundColor: Colors.transparent,
                            insetPadding: const EdgeInsets.all(0),
                            child: GestureDetector(
                              onTap: () {
                                Get.back();
                              },
                              child: InteractiveViewer(
                                child: CustomImage(
                                  isTouchable: false,
                                  imageUrl: widget.isQuestionPage
                                      ? widget.imageUrl.replaceAll(
                                          'QuestionThumbnail',
                                          'QuestionPicture')
                                      : widget.imageUrl,
                                  headers: ApplicationConstants.XAPIKEY,
                                ),
                              ),
                            ),
                          );
                        },
                      );
                    }
                  : null,
              child: snapshot.data!,
            ),
          );
        }
      },
    );
  }
}
