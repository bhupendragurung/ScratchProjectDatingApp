import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { of, map, take } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/Pagination';
import { User } from '../_models/user';

import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MembersService {
  members:Member[]=[];
baseUrl=environment.apiUrl;
memberCache=new Map();
user: User|undefined;
userParams:UserParams|undefined;
  constructor(private http:HttpClient,private accountService:AccountService) { 
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next : user =>{
        if(user){
          this.userParams=new UserParams(user);
          this.user=user;
        }
      }
    })
  }
getUserParams(){
  return this.userParams;

}
resetUserParams(){
  if(this.user){
    this.userParams= new UserParams(this.user);
    return this.userParams;
  }
  return;
}
setUserParams(userParams:UserParams){
this.userParams=userParams;
}
  getMembers(UserParams:UserParams){
const response=this.memberCache.get(Object.values(UserParams).join('-'));
if(response){return of(response);}

    let params = getPaginationHeaders(UserParams.pageNumber, UserParams.pageSize);
    params= params.append('minAge',UserParams.minAge);
    params= params.append('maxAge',UserParams.maxAge);
    params= params.append('gender',UserParams.gender);
    params= params.append('orderBy',UserParams.orderBy);
    // if(this.members.length>0) return of(this.members);
    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params,this.http).pipe(
      map(response =>{

        this.memberCache.set(Object.values(UserParams).join('-'),response);
        return response;
      })
    )
  }

  

  getMember(username:string){
    const member=[...this.memberCache.values()].reduce(
      (arr,elem)=>arr.concat(elem.result),[]
    ).find((member:Member)=> member.username == username);

    if(member) return of(member)
    return this.http.get<Member>(this.baseUrl+'users/'+username);
  }
  updateMember(member:Member){
    return this.http.put(this.baseUrl+'users',member).pipe(
      map(()=>{
        const index=this.members.indexOf(member);
        this.members[index]={...this.members[index],...member}
      })
    );
  }
  setMainPhoto(PhotoId:number){
   return this.http.put(this.baseUrl+'users/set-main-photo/' +PhotoId.toString(),{});
  }
  DeletePhoto(photoId:number){
   return this.http.delete(this.baseUrl+'users/delete-photo/' +photoId);

  }
  addLike(username:string){
    return this.http.post(this.baseUrl +'likes/' + username,{});
  }
  getLikes(predicate:string,pageNumber:number,pageSize:number){
    let params=getPaginationHeaders(pageNumber,pageSize);
    params= params.append('predicate',predicate);

    return getPaginatedResult<Member[]>(this.baseUrl + 'likes',params,this.http);
  }
}
