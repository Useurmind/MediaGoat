import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import * as AdminStore from "../store/Admin";
import { ApplicationState } from '../store';

interface AdminState {

}

type AdminProps =
    AdminStore.AdminState
    & typeof AdminStore.actionCreators
    & RouteComponentProps<{}>;


class Admin extends React.Component<AdminProps, AdminState> {
    constructor(props: AdminProps) {
        super(props);

        this.state = {
        };
    }

    public render() {
        return <div>
            <button onClick={e => this.props.startIndexRun()}>Start Index Run</button>
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.admin,                                       // Selects which state properties are merged into the component's props
    AdminStore.actionCreators                // Selects which action creators are merged into the component's props
)(Admin) as typeof Admin;