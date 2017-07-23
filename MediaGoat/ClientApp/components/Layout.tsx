import * as React from 'react';
import { NavMenu } from './NavMenu';
import Player from "./Player";

export class Layout extends React.Component<{}, {}> {
    public render() {
        return <div className='container-fluid'>
            <div className='row'>
                <div className='col-sm-3'>
                    <NavMenu />
                </div>
                <div className='col-sm-9'>
                    {this.props.children}
                </div>
            </div>
            <div className="row">
                <div className='col-sm-3'>
                </div>
                <div className='col-sm-9'>
                    <div className="navbar navbar-default navbar-fixed-bottom">
                        <Player />
                        </div>
                </div>
            </div>
        </div>;
    }
}
