import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Sidebar } from '../sidebar/sidebar';
import { Header } from '../header/header';
import { StoreIdentity } from '../store-identity/store-identity';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [RouterOutlet, Sidebar, Header, StoreIdentity],
  templateUrl: './shell.html',
  styleUrl: './shell.scss'
})
export class Shell {}