import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as SongsStore from '../store/Songs';
import * as PlayerStore from "../store/Player";

interface SongsUiState {
    searchString: string
}

type SongsProps =
    SongsStore.SongsState
    & typeof SongsStore.actionCreators
    & typeof PlayerStore.actionCreators
    & RouteComponentProps<{}>;

class Songs extends React.Component<SongsProps, SongsUiState> {
    constructor(props: SongsProps) {
        super(props);

        this.state = {
            searchString: ""
        };
    }


    public render() {
        return <div>
            <h1>Songs</h1>

            <p>Search for songs.</p>
            
                <div className="form-group">
                    <label htmlFor="searchString">Search text (in Artist, Title, Album):</label>
                    <input type="text"
                        className="form-control"
                        id="searchString"
                        onChange={(e) => this.setState({ searchString: e.target.value })}
                        onKeyPress={e => { if (e.key === "Enter") this.props.searchSongs(this.state.searchString); }} />
                </div>
                <button type="button" className="btn btn-default" onClick={() => { this.props.searchSongs(this.state.searchString) }}>Search</button>

            <table className='table'>
                <thead>
                    <tr>
                        <th></th>
                        <th>Title</th>
                        <th>Artist</th>
                        <th>Album</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        this.props.songs.map(s =>
                            <tr key={s.guid}>
                                <td>
                                    <button onClick={e => this.props.playSong(s)}>
                                        <i className="fa fa-play" aria-hidden="true"></i>
                                    </button>
                                </td>
                                <td>{s.title}</td>
                                <td>{s.artist}</td>
                                <td>{s.album}</td>
                            </tr>
                        )
                    }
                </tbody>
            </table>
        </div>;
    }
}

// Wire up the React component to the Redux store
export default connect(
    (state: ApplicationState) => state.songs,                                       // Selects which state properties are merged into the component's props
    { ...SongsStore.actionCreators, ...PlayerStore.actionCreators }                 // Selects which action creators are merged into the component's props
)(Songs) as typeof Songs;