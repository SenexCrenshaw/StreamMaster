import { QueryHookResult } from '@lib/apiDefs';
import store, { RootState } from '@lib/redux/store';
import { useAppDispatch, useAppSelector } from '@lib/redux/hooks';
import { clear, clearByTag, setField, setIsForced, setIsLoading } from './GetIntroPlayListsSlice';
import { useCallback,useEffect } from 'react';
import { useSMContext } from '@lib/context/SMProvider';
import { fetchGetIntroPlayLists } from './GetIntroPlayListsFetch';
import {FieldData, CustomPlayList } from '@lib/smAPI/smapiTypes';

interface ExtendedQueryHookResult extends QueryHookResult<CustomPlayList[] | undefined> {}
interface Result extends ExtendedQueryHookResult {
  Clear: () => void;
  ClearByTag: (tag: string) => void;
  SetField: (fieldData: FieldData) => void;
  SetIsForced: (force: boolean) => void;
  SetIsLoading: (isLoading: boolean, query: string) => void;
}
const useGetIntroPlayLists = (): Result => {
  const { isSystemReady } = useSMContext();
  const dispatch = useAppDispatch();
  const isForced = useAppSelector((state) => state.GetIntroPlayLists.isForced ?? false);

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
    return state.GetIntroPlayLists.data;
  };
const data = useAppSelector(selectData);

const selectError = (state: RootState) => {
    return state.GetIntroPlayLists.error;
  };
const error = useAppSelector(selectError);

const selectIsError = (state: RootState) => {
    return state.GetIntroPlayLists.isError;
  };
const isError = useAppSelector(selectIsError);

const selectIsLoading = (state: RootState) => {
    return state.GetIntroPlayLists.isLoading;
  };
const isLoading = useAppSelector(selectIsLoading);


  useEffect(() => {
    const state = store.getState().GetIntroPlayLists;
    if (data === undefined && state.isLoading !== true && state.isForced !== true) {
      SetIsForced(true);
    }
  }, [SetIsForced, data]);

useEffect(() => {
    if (!isSystemReady) return;
  const state = store.getState().GetIntroPlayLists;
  if (state.isLoading) return;
  if (data !== undefined && !isForced) return;

  SetIsLoading(true);
  dispatch(fetchGetIntroPlayLists());
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

export default useGetIntroPlayLists;
