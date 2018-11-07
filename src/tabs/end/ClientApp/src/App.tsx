import * as React from 'react';
import { TeamsComponentContext, ThemeStyle } from 'msteams-ui-components-react';
import { TabExample } from './TabExample';
import { ConfigTab } from './ConfigTab';
import { BrowserRouter as Router, Route } from "react-router-dom";

class App extends React.Component {

  public render() {
    return (
      <TeamsComponentContext
        fontSize={16}
        theme={ThemeStyle.Light}>
        <Router>
          <div>
            <Route path="/" exact render={() => <TabExample />} />
            <Route path="/config" render={() => <ConfigTab />} />
            <Route path="/project/:id" render={(props) => <div><h1>{props.match.params.id}</h1></div>} />
            </div>
        </Router>
      </TeamsComponentContext>
    );
  }
}

export default App;
