import { QueryHookResult } from '@lib/apiDefs';
import store, { RootState } from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, clearByTag, setField, setIsForced, setIsLoading } from './GetM3UFileNamesSlice';
import { useCallback,useEffect } from 'react';
import { useSMContext } from '@lib/context/SMProvider';
import { fetchGetM3UFileNames } from './GetM3UFileNamesFetch';
import {FieldData,  } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<string[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  ClearByTag: (tag: string) => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetM3UFileNames = (): Result => {
  const { isSystemReady } = useSMContext();
  const dispatch = useAppDispatch();
  const isForced = useAppSelector((state) => state.GetM3UFileNames.isForced ?? false);

  const SetIsForced = useCallback(
    (forceRefresh: boolean): void => {
      dispatch(setIsForced({ force: forceRefresh }));
    },
    [dispatch]
  );
  const ClearByTag = useCallback(
    (tag: string): void => {
      dispatch(clearByTag({tag: tag }));
    },
    [dispatch]
  );



const SetIsLoading = useCallback(
  (isLoading: boolean): void => {
    dispatch(setIsLoading({ isLoading: isLoading }));
  },
  [dispatch]
);
const selectData = (state: RootState) => {
    return state.GetM3UFileNames.data;
  };
const data = useAppSelector(selectData);

const selectError = (state: RootState) => {
    return state.GetM3UFileNames.error;
  };
const error = useAppSelector(selectError);

const selectIsError = (state: RootState) => {
    return state.GetM3UFileNames.isError;
  };
const isError = useAppSelector(selectIsError);

const selectIsLoading = (state: RootState) => {
    return state.GetM3UFileNames.isLoading;
  };
const isLoading = useAppSelector(selectIsLoading);


  useEffect(() => {
    const state = store.getState().GetM3UFileNames;
    if (data === undefined && state.isLoading !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [SetIsForced, data]);

useEffect(() => {
    if (!isSystemReady) return;
  const state = store.getState().GetM3UFileNames;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetM3UFileNames());
}, [SetIsLoading, data, dispatch, isForced, isSystemReady]);

const SetField = (fieldData: FieldData): void => {
  dispatch(setField({ fieldData: fieldData }));
};

const Clear = (): void => {
  dispatch(clear());
};

return {
  Clear,
  ClearByTag,
  data,
  error,
  isError,
  isLoading,
  SetField,
  SetIsForced,
  SetIsLoading
};
};

export default useGetM3UFileNames;
