import * as React from 'react';
import { Link, RouteComponentProps } from 'react-router-dom';
import { connect } from 'react-redux';
import { ApplicationState } from '../store';
import * as SongsStore from '../store/Songs';

interface SongsUiState {
    searchString: string
}

type SongsProps =
    SongsStore.SongsState
    & typeof SongsStore.actionCreators
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

            <input type="text" onChange={(e) => this.setState({ searchString: e.target.value })} />
            <button onClick={() => { this.props.searchSongs(this.state.searchString) }}>Search</button>

            <table className='table'>
                <thead>
                    <tr>
                        <th>Title</th>
                        <th>Artist</th>
                        <th>Album</th>
                    </tr>
                </thead>
                <tbody>
                    {
                        this.props.songs.map(s =>
                            <tr key={s.title}>
                                <td>{s.title}</td>
                                <td>{s.artist}</td>
                                <td>{s.album}</td>
                                <td><audio controls>
                                    <source src={s.url} type="audio/mpeg" />
                                </audio>
                                </td>
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
    (state: ApplicationState) => state.songs, // Selects which state properties are merged into the component's props
    SongsStore.actionCreators                 // Selects which action creators are merged into the component's props
)(Songs) as typeof Songs;