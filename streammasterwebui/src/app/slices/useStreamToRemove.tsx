import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../store';
import { setStreamToRemove as setStreamToRemoveInternal } from './streamToRemoveSlice';

export const useStreamToRemove = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setStreamToRemove = (toRemove: string) => {
    dispatch(setStreamToRemoveInternal({
      toRemove: toRemove,
      typename
    }));
  };

  const streamToRemove = useSelector((rootState: RootState) => rootState.streamToRemove[typename]);

  return { setStreamToRemove, streamToRemove };
};
