import SMTasksDataSelector from '@components/smtasks/SMTasksDataSelector';
import './ball-beat.css';

const SMLoader = () => {
  return (
    <div className="sm-loader">
      <div className="sm-modal flex flex-column justify-content-center align-items-center align-content-center">
        <SMTasksDataSelector />
        <div className="la-ball-beat la-2x w-full flex justify-content-center align-items-center z-10 surface-ground">
          <div></div>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
          <div></div>
        </div>
      </div>
    </div>
  );
};

export default SMLoader;
