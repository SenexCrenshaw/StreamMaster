import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetSMChannelStreamsSlice';
import { useCallback,useEffect } from 'react';
import { fetchGetSMChannelStreams } from './GetSMChannelStreamsFetch';
import {FieldData, SMStreamDto } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<SMStreamDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetSMChannelStreams = (): Result => {
  const dispatch = useAppDispatch();
  const data = useAppSelector((state) => state.GetSMChannelStreams.data);
  const error = useAppSelector((state) => state.GetSMChannelStreams.error ?? '');
  const isError = useAppSelector((state) => state.GetSMChannelStreams.isError?? false);
  const isForced = useAppSelector((state) => state.GetSMChannelStreams.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetSMChannelStreams.isLoading ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean, query?: string): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );

  const SetIsLoading = useCallback(
    (isLoading: boolean): void => {
      dispatch(setIsLoading({ isLoading: isLoading }));
    },
    [dispatch]
  );

useEffect(() => {
  const state = store.getState().GetSMChannelStreams;

  if (data === undefined && state.isLoading !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch]);


const SetField = (fieldData: FieldData): void => {
  dispatch(setField({ fieldData: fieldData }));
};

const Clear = (): void => {
  dispatch(clear());
};

return {
  data,
  error,
  isError,
  isLoading,
  Clear,
  SetField,
  SetIsForced,
  SetIsLoading
};
};

export default useGetSMChannelStreams;
