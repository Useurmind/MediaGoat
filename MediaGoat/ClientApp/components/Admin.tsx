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

    componentDidMount() {
        this.props.startGettingIndexState();
    }

    componentWillUnmount() {
        this.props.stopGettingIndexState();
    }

    public render() {
        let indexResult = this.props.indexResult;

        let indexStateIcon = null;
        let buttonClass = null;
        let alertClass = null;
        let alertText = indexResult.message;
        switch (indexResult.status) {
            case AdminStore.IndexState.None:
                indexStateIcon = <i className="fa fa-ban"></i>;
                buttonClass = "btn-info";
                alertClass = "alert-info";
                break;
            case AdminStore.IndexState.Running:
                indexStateIcon = <i className="fa fa-circle-o-notch fa-spin fa-fw"></i>;
                buttonClass = "btn-info";
                alertClass = "alert-info";
                break;
            case AdminStore.IndexState.Success:
                indexStateIcon = <i className="fa fa-check" aria-hidden="true" ></i>
                buttonClass = "btn-success";
                alertClass = "alert-success";
                break;
            case AdminStore.IndexState.Failed:
                indexStateIcon = <i className="fa fa-times"></i>
                buttonClass = "btn-danger";
                alertClass = "alert-error";
                break;
        }

        if (indexResult.startTime) {
            alertText = `${alertText}. Started at ${indexResult.startTime.toLocaleString()}.`;
        }
        if (indexResult.stopTime) {
            alertText = `${alertText} Stopped at ${indexResult.stopTime.toLocaleString()}.`;
        }

        let indexAlert = <div className={`alert ${alertClass}`} role="alert">
            {alertText}
        </div>



        return <div>
            <h1>Admin functions</h1>
            <div className="panel panel-default">
                <div className="panel-heading">
                    Indexing
                </div>
                <div className="panel-body">
                    <div className="form-group">
                        <label htmlFor="triggerIndexing">Index media files:</label>
                        <div className="input-group" id="triggerIndexing">
                            <span className="input-group-btn">
                                <button onClick={e => this.props.startIndexRun()} className="btn btn-default">Start Index Run</button>
                                <button className={`btn ${buttonClass}`}>
                                    {indexStateIcon}
                                </button>
                            </span>
                        </div>
                    </div>
                    
                    {indexAlert}
                </div>
            </div>
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.admin,                                       // Selects which state properties are merged into the component's props
    AdminStore.actionCreators                // Selects which action creators are merged into the component's props
)(Admin) as typeof Admin;