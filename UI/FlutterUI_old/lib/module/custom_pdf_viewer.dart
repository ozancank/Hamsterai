// import 'package:dio/dio.dart';
// import 'package:flutter/material.dart';
// import 'package:syncfusion_flutter_pdfviewer/pdfviewer.dart';

// class CustomPdfViewer extends StatefulWidget {
//   final String pdfUrl;
//   final Map<String, String> headers;
//   final bool isTouchable;

//   const CustomPdfViewer({
//     required this.pdfUrl,
//     required this.headers,
//     this.isTouchable = true,
//     super.key,
//   });

//   @override
//   CustomPdfViewerState createState() => CustomPdfViewerState();
// }

// class CustomPdfViewerState extends State<CustomPdfViewer> {
//   late Future<PdfDocument> _pdfFuture;

//   @override
//   void initState() {
//     super.initState();
//     _pdfFuture = _loadPdf();
//   }

//   Future<PdfDocument> _loadPdf() async {
//     try {
//       final dio = Dio();
//       final response = await dio.get<ResponseBody>(
//         widget.pdfUrl,
//         options: Options(
//           headers: widget.headers,
//           responseType: ResponseType.bytes,
//         ),
//       );

//       final bytes = response.data;
//       return PdfDocument.openData(bytes);
//     } catch (e) {
//       print('Error loading PDF: $e');
//       rethrow;
//     }
//   }

//   @override
//   Widget build(BuildContext context) {
//     return FutureBuilder<PdfDocument>(
//       future: _pdfFuture,
//       builder: (BuildContext context, AsyncSnapshot<PdfDocument> snapshot) {
//         if (snapshot.connectionState == ConnectionState.waiting) {
//           return const Center(
//             child: CircularProgressIndicator(color: Colors.blue),
//           );
//         } else if (snapshot.hasError) {
//           return const Center(
//             child: Text('Error loading PDF'),
//           );
//         } else if (snapshot.hasData) {
//           return SfPdfViewer.network(
//             widget.pdfUrl,
//             headers: widget.headers,
//           );
//         } else {
//           return const Center(
//             child: Text('No PDF available'),
//           );
//         }
//       },
//     );
//   }
// }
