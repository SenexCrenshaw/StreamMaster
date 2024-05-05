import React, { CSSProperties, useState } from 'react';
import { ClipLoader, GridLoader } from 'react-spinners';

const Loader = () => {
  let [loading, setLoading] = useState(true);
  let [color, setColor] = useState('#ffffff');

  const override: CSSProperties = {
    display: 'block',
    margin: '0 auto',
    width: '100%',
    height: '100%',
    borderColor: 'red'
  };

  return (
    <div className="flex align-items-center justify-content-center">
      <GridLoader color={color} loading={loading} cssOverride={override} size={150} aria-label="Loading Spinner" data-testid="loader" />
    </div>
  );
};

export default Loader;
