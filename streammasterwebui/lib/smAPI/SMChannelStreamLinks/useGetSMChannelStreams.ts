import { QueryHookResult } from '@lib/apiDefs';
import store from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, setField, setIsForced, setIsLoading } from './GetSMChannelStreamsSlice';
import { useCallback,useEffect } from 'react';
import {FieldData, SMStreamDto,GetSMChannelStreamsRequest } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<SMStreamDto[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetSMChannelStreams = (params?: GetSMChannelStreamsRequest): Result => {
  const dispatch = useAppDispatch();
  const param = JSON.stringify(params);
  const data = useAppSelector((state) => state.GetSMChannelStreams.data[param]);
  const error = useAppSelector((state) => state.GetSMChannelStreams.error[param] ?? '');
  const isError = useAppSelector((state) => state.GetSMChannelStreams.isError[param] ?? false);
  const isForced = useAppSelector((state) => state.GetSMChannelStreams.isForced ?? false);
  const isLoading = useAppSelector((state) => state.GetSMChannelStreams.isLoading[param] ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );

  const SetIsLoading = useCallback(
    (isLoading: boolean, param: string): void => {
      dispatch(setIsLoading({ isLoading: isLoading, param: param }));
    },
    [dispatch]
  );

useEffect(() => {
  if (param === undefined) return;
  const state = store.getState().GetSMChannelStreams;

  if (data === undefined && state.isLoading[param] !== true && state.isForced !== true) {
    SetIsForced(true);
  }
}, [SetIsForced, data, dispatch, param]);


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
