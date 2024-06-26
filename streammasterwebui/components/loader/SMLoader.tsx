import SMTasksDataSelector from '@components/smtasks/SMTasksDataSelector';
// import { useSMContext } from '@lib/signalr/SMProvider';
import './ball-beat.css';
// import './ball-spin-clockwise-fade.css';

const SMLoader = () => {
  // const { isTaskRunning } = useSMContext();
  // Logger.debug('SMLoader');

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

  // return (
  //   <div className="sm-loader">
  //     <div className="flex flex-column justify-content-center align-items-center align-content-center">
  //       <div className="la-ball-spin-clockwise-fade la-3x">
  //         <div></div>
  //         <div></div>
  //         <div></div>
  //         <div></div>
  //         <div></div>
  //         <div></div>
  //         <div></div>
  //         <div></div>
  //       </div>
  //     </div>
  //   </div>
  // );
};

export default SMLoader;
