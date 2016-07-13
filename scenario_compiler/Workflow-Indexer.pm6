

class Workflow-Indexer {

# put paths (cwd, args, config, ?) into @paths-to-search
# Index-Workflows-in-paths

    # for each path to search
        # add folders to paths to search
        # for each file matching .*\.cs
            # parse for class name.
            #  If it is a test workflow, grab method names.
            #  Otherwise, skip ahead until... you know, it's probably better to
            #  ignore this wrinkle and simply grab all public void methods.

}
