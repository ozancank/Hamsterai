import 'package:flutter/material.dart';
import 'package:mobile/core/constants/app_constant.dart';
import 'package:mobile/pages/common/common_appbar.dart';
import 'package:mobile/pages/common/common_appbar2.dart';
import 'package:syncfusion_flutter_pdfviewer/pdfviewer.dart';

class HomeworkPdfViewer extends StatefulWidget {
  final String filePath;
  const HomeworkPdfViewer({super.key, required this.filePath});

  @override
  State<HomeworkPdfViewer> createState() => _HomeworkDetailViewState();
}

class _HomeworkDetailViewState extends State<HomeworkPdfViewer> {
  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar:  CommonAppbar2(title: 'Ödev Detay'),
      backgroundColor: const Color(0xFFF6F7F8),
      body: SfPdfViewer.network(
          headers: ApplicationConstants.XAPIKEY,
          '${ApplicationConstants.APIBASEURL}/homework/${widget.filePath}'),
    );
  }
}
