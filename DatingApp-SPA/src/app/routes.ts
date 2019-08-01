import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MemberDetailResolver } from './_resolvers/member-detail.resolver';
import { AuthGuard } from './_guards/auth.guard';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { MemberDetailComponent } from './members/member-detail/member-detail.component';

export const appRoutes: Routes = [
   {path: '', component: HomeComponent},
   {path: 'home', component: HomeComponent},
   {
      path: '', 
      runGuardsAndResolvers: 'always',
      canActivate: [AuthGuard],
      children: [
         {path: 'members', component: MemberListComponent, resolve: {users: MemberListResolver}},
         {path: 'members/:id', component: MemberDetailComponent, resolve: {users: MemberDetailResolver}},
         {path: 'messages', component: MessagesComponent},
         {path: 'lists', component: ListsComponent}

      ]
   },
   {path: '**', redirectTo : 'home', pathMatch: 'full'}
]