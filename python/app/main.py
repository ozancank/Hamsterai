import os
from aiohttp import web
import asyncio
from ocr import get_text

async def handle_post(request):
  try:
    data = await request.json()
    file_name = data.get('fileName')

    if not file_name:
      return web.json_response({'message': f'Soru dosyası bulunamadı: {file_path}'}, status=404)

    file_path = os.path.join(file_name)
    if not os.path.exists(file_path):
      return web.json_response({'message': f'Soru dosyası bulunamadı: {file_path}' }, status=404)

    text = get_text(file_path)

    return web.json_response({'message': text})
  except asyncio.TimeoutError:
    return web.json_response({'message': 'timeout'}, status=408)
  except Exception as e:
    return web.json_response({'message': str(e)}, status=500)

async def init_app():
  app = web.Application()
  app.router.add_post('/image-ocr', handle_post)
  return app

if __name__ == '__main__':
  app = asyncio.run(init_app())
  web.run_app(app, port=5006)