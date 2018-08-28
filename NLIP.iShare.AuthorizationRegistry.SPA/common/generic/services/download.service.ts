import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class DownloadService {
  constructor(private http: HttpClient) {}

  get(url: string) {
    return this.http.get(url, { responseType: 'blob', observe: 'response' }).pipe(
      map(r => {
        const nameHeader = r.headers.get('content-disposition');
        if (!nameHeader || nameHeader.match(/filename=(.+)/).length < 2) {
          throw new Error('Filename must be provided in a Content-Disposition header.');
        }
        const filename = nameHeader.match(/filename=(.+)/)[1];
        return { filename: filename, blob: r.body };
      })
    );
  }

  performBrowserDownload(filename: string, blob: Blob): void {
    if (!document) {
      throw Error('Must have access to DOM');
    }
    let link = document.getElementById('download');
    if (!link) {
      link = document.createElement('a');
      link.setAttribute('id', 'download');
      link.setAttribute('style', 'visibility:hidden');
      // in firefox link must be a part of DOM
      document.body.appendChild(link);
    }
    const final = new Blob([blob], { type: 'text/octet-stream' });
    link.setAttribute('href', window.URL.createObjectURL(final));
    link.setAttribute('download', filename);
    link.click();
  }
}
