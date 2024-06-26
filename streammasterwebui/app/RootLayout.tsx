import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { useShowSelected } from '@lib/redux/hooks/showSelected';
import useGetStreamGroup from '@lib/smAPI/StreamGroups/useGetStreamGroup';
import { GetStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import { useCallback, useEffect, useRef } from 'react';
import { Outlet } from 'react-router-dom';
import { RootSideBar } from './RootSideBar';

export const RootLayout = (): JSX.Element => {
  const { setIsTrue } = useIsTrue('isSimple');
  const { setShowSelected } = useShowSelected('SchedulesDirectStationDataSelector');
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const sgquery = useGetStreamGroup({ SGName: 'ALL' } as GetStreamGroupRequest);
  const initialized = useRef(false);

  useEffect(() => {
    if (selectedStreamGroup === undefined && sgquery.data !== undefined) {
      setSelectedStreamGroup(sgquery.data);
    }
  }, [selectedStreamGroup, setSelectedStreamGroup, sgquery.data]);

  const setPersistTrue = useCallback(() => {
    const persistKey = 'persist:isTrue';
    const persistData = localStorage.getItem(persistKey);

    if (persistData === null) {
      setShowSelected(true);
      setIsTrue(true);
    } else {
      try {
        const parsedData = JSON.parse(persistData);
        if (!parsedData.hasOwnProperty('isSimple')) {
          setIsTrue(true);
        }
      } catch (error) {
        setIsTrue(true);
      }
    }
  }, [setIsTrue, setShowSelected]);

  useEffect(() => {
    if (!initialized.current) {
      setPersistTrue();
      initialized.current = true;
    }
  }, [setPersistTrue]);

  return (
    <div className="flex max-h-screen p-fluid">
      <RootSideBar />
      <div className="main-outlet flex flex-column w-full">
        <Outlet />
      </div>
    </div>
  );
};
