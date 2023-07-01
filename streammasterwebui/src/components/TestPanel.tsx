import React from "react";

import EPGSelector from "./EPGSelector";

const TestPanel = () => {

  return (
    <EPGSelector />
  );
}

TestPanel.displayName = 'TestPanel';
TestPanel.defaultProps = {

};

// type TestPanelProps = {

// };

export default React.memo(TestPanel);
