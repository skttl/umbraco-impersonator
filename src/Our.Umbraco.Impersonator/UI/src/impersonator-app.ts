import { LitElement, html } from "lit";
import { customElement } from "lit/decorators.js";
import { UmbElementMixin } from '@umbraco-cms/backoffice/element-api';

@customElement('impersonator-app')
export default class ImpersonatorApp extends UmbElementMixin(LitElement) {

	constructor() {
    super();
	}

  endImpersonation() {
    // todo
  }
  
  impersonateUser() {
    // todo
  }

  setUserToImpersonate(event: Event) {
    if ((event.target as HTMLSelectElement).value) {
      this.userToImpersonate = (event.target as HTMLSelectElement).value;
    }
  }

  isImpersonating = false; // todo;
  users = []; // todo:
  userToImpersonate = '';

	render() {
    return this.isImpersonating ?
      html`
			<uui-box headline="Impersonator">
				<p>You are impersonating the current user</p>
				<uui-button look="primary" label="End impersonation" @click=${this.endImpersonation}></uui-button>
			</uui-box>
		` :
      html`
			<uui-box headline="Impersonator">
        <select class="umb-dropdown flx-g1 mb0" @change=${this.setUserToImpersonate}>
          <option>todo: get the users</option>
        </select>
				<uui-button look="primary" label="Impersonate" @click=${this.impersonateUser}></uui-button>
			</uui-box>
      `;
	}
}

declare global {
	interface HTMLElementTagNameMap {
		'impersonator-app': ImpersonatorApp;
	}
}