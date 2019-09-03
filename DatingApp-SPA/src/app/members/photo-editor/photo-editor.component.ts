import { AlertifyService } from './../../_services/alertify.service';
import { UserService } from './../../_services/user.service';
import { AuthService } from './../../_services/auth.service';
import { environment } from './../../../environments/environment';
import { Photo } from './../../_models/Photo';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() photos: Photo[];
  // event emitter from angular core, please double check
  // @Output() getMemberPhotoChange = new EventEmitter<string>();
  photoUrl: string;
  currentMain: Photo;
  public uploader: FileUploader;
  public hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;

  constructor(private authService: AuthService,
    private userService: UserService, private alertifySvc: AlertifyService) { }

  ngOnInit() {
    this.initializeUploader();
  }

  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024
    });

    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
        if (response) {
         const res: Photo =  JSON.parse(response);
         const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain
         };

         if (photo.isMain) {
          this.authService.changeMemberPhotoUrl(photo.url);
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
         }

         this.photos.push(photo);
        }
    };

  }

  SetMainPhoto(photo: Photo) {
    this.userService.setMainPhoto(photo.id, this.authService.decodedToken.nameid)
    .subscribe((response) => {
      this.alertifySvc.success('Photo Set as main photo');
      this.currentMain = this.photos.filter(p => p.isMain === true)[0];
      this.currentMain.isMain = false;
      photo.isMain = true;
      // this.getMemberPhotoChange.emit(photo.url);
       this.authService.changeMemberPhotoUrl(photo.url);
       this.authService.currentUser.photoUrl = photo.url;
       localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
    }, (error) => {
      this.alertifySvc.error('Some error happened');
    });
  }

  deletePhoto(id: number) {
    this.alertifySvc.confirm('Do you want to delete ?', () => {
      this.userService.deletePhoto(id, this.authService.decodedToken.nameid).subscribe(() => {
        this.photos.splice(this.photos.findIndex(p => p.id === id), 1);
        this.alertifySvc.success('Photo is deleted');
      }, (error) => {
        this.alertifySvc.success('Failed to delete'); 
      });
    });
  }

}
