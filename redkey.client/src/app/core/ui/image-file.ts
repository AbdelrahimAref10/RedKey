/**
 * Reads an image file as a data-URL (base64) for the API. Photos are scaled down to `maxSize`
 * and re-encoded as JPEG first, so uploads stay small and fast; SVG icons are sent as they are.
 */
export async function readImageAsDataUrl(file: File, maxSize = 1600, quality = 0.85): Promise<string> {
  if (file.type === 'image/svg+xml') return readAsDataUrl(file);

  const bitmap = await createImageBitmap(file);
  const scale = Math.min(1, maxSize / Math.max(bitmap.width, bitmap.height));
  const width = Math.round(bitmap.width * scale);
  const height = Math.round(bitmap.height * scale);

  const canvas = document.createElement('canvas');
  canvas.width = width;
  canvas.height = height;
  canvas.getContext('2d')!.drawImage(bitmap, 0, 0, width, height);
  bitmap.close();

  // PNG keeps transparency (icons); everything else becomes a compact JPEG.
  return file.type === 'image/png' && maxSize <= 512
    ? canvas.toDataURL('image/png')
    : canvas.toDataURL('image/jpeg', quality);
}

function readAsDataUrl(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => resolve(reader.result as string);
    reader.onerror = () => reject(reader.error);
    reader.readAsDataURL(file);
  });
}
